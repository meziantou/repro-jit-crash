# .NET 11 RC2 JIT crash: SIGSEGV in `Compiler::fgCreateGCPoll`

On .NET 11.0.0-rc.2 the process crashes with SIGSEGV (exit code 139) while the tiered-compilation background thread
recompiles a method at tier 1 with dynamic PGO data. The crash is inside `libclrjit`, no managed frame is involved.
It does not happen on .NET 10, with `DOTNET_TieredPGO=0`, with `DOTNET_TieredCompilation=0`, or when the method has
`[MethodImpl(MethodImplOptions.AggressiveOptimization)]`.

The repro is one file of about 120 lines: [Repro/Program.cs](Repro/Program.cs). It was reduced from a test262 runner
for a JavaScript engine (from [Meziantou.Framework](https://github.com/meziantou/Meziantou.Framework), 138,000 lines)
by deleting everything that is not needed to keep the crash. The method that gets compiled when the JIT crashes is
`RunMode`.

## Reproduce

Requirements: the .NET SDK `11.0.100-rc.2.26504.105` (pinned by [global.json](global.json)). No package reference.

```bash
dotnet build Repro -c Release
./Repro/bin/Release/net11.0/Repro
echo $?    # 139: the process crashed (SIGSEGV)
```

or run `./repro.sh`, which does this three times, then once with `DOTNET_TieredPGO=0`.

Expected: `Done`, exit code 0.
Actual: the process dies after about half a second, exit code 139, no output.

Tested: macOS arm64, runtime `11.0.26.50505 @Commit: 820230c386db5b0106c5430b3e5aee92a7826bf5`, crashes on every run.
The original (large) repro also crashed on a Linux x64 CI runner (GitHub Actions) with the same stack.

| Setup | Result |
| --- | --- |
| net11.0 (default) | crash |
| net10.0 (`-p:TargetFramework=net10.0`, restore first) | OK |
| `DOTNET_TieredPGO=0` | OK |
| `DOTNET_TieredCompilation=0` | OK |
| `[MethodImpl(MethodImplOptions.AggressiveOptimization)]` on `RunMode` | OK |
| `DOTNET_TieredPGO_InstrumentOnlyHotCode=0` | crash |
| `DOTNET_TC_CallCountThreshold=1` | crash |

## Native stack of the crashing thread

Captured with `DOTNET_DbgEnableMiniDump=1 DOTNET_EnableCrashReport=1`:

```
libsystem_platform.dylib  _sigtramp
libclrjit.dylib           Compiler::fgCreateGCPoll(Compiler::GCPollType, BasicBlock*) +0x2a4
libclrjit.dylib           Compiler::fgInsertGCPolls() +0x54
libclrjit.dylib           Phase::Run()
libclrjit.dylib           Compiler::compCompile(void**, unsigned int*, JitFlags*)
libclrjit.dylib           Compiler::compCompileHelper(...)
libclrjit.dylib           Compiler::compCompileAfterInit(...)
libclrjit.dylib           jitNativeCode(...)
libclrjit.dylib           CILJit::compileMethod(...)
libcoreclr.dylib          UnsafeJitFunction(...)
libcoreclr.dylib          MethodDesc::JitCompileCodeLocked(...)
libcoreclr.dylib          MethodDesc::JitCompileCode(...)
libcoreclr.dylib          MethodDesc::PrepareCode(...)
libcoreclr.dylib          TieredCompilationManager::CompileCodeVersion(...)
libcoreclr.dylib          TieredCompilationManager::DoBackgroundWork(...)
libcoreclr.dylib          TieredCompilationManager::BackgroundWorkerStart()
```

## What the repro needs

Found while reducing (removing any of these makes the crash disappear):

- `RunMode` loops over an iterator method (`foreach` over a `yield return` method). Over an array it does not crash.
- The loop calls an instance method (`JsRealm.Execute`) that has a `using` scope (a `try`/`finally`) and that is
  inlined into `RunMode` together with `Compile` (a `new FunctionTemplate { ... }` with 11 required properties set in
  one initializer) and `Run` (which calls the `Frame` constructor).
- `Frame`'s constructor allocates a `JsValue[]` (`JsValue` is a struct whose only field is an object reference) and
  copies a `ReadOnlySpan<JsValue>` (an empty collection expression `[]`) into it with `Span.CopyTo`.
- `JsEngine.CreateRealm` has a `using` scope too, and it uses a `[ThreadStatic]` field.
- `Parallel.For` runs `RunMode` 500,000 times, so that it reaches tier 1 with dynamic PGO data on several threads.
- The number of properties of `FunctionTemplate` matters: removing a property of the initializer (even an unused
  one) makes the crash disappear, so the size of the inlinee seems to matter (inlining budget).

A stub of the same shape written from scratch, with less code in the inlinee, did not crash.

A JIT dump of the failing tier-1 compile (checked JIT, `DOTNET_JitDump`) would pinpoint the faulting block; I only
have the release JIT.
