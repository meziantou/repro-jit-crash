# .NET 11 RC2 JIT crash: SIGSEGV in `Compiler::fgCreateGCPoll`

On .NET 11.0.0-rc.2 the process crashes with SIGSEGV (exit code 139) while the tiered-compilation background thread
recompiles a method at tier 1 with dynamic PGO data. The crash is inside `libclrjit`, no managed frame is involved.
It does not happen on .NET 10, with `DOTNET_TieredPGO=0`, with `DOTNET_TieredCompilation=0`, or when the method has
`[MethodImpl(MethodImplOptions.AggressiveOptimization)]`.

The repro is small in code but still depends on a large library: `Repro/Program.cs` (20 lines) creates a
`JsEngine` and a realm of a JavaScript engine written in C# (extracted from
[Meziantou.Framework](https://github.com/meziantou/Meziantou.Framework)) and executes a script, 500,000 times in
parallel. The method that gets compiled when the JIT crashes is `RunMode` in `Program.cs`.

## Reproduce

Requirements: the .NET SDK `11.0.100-rc.2.26504.105` (pinned by [global.json](global.json)). There is no package
reference.

```bash
dotnet build Repro -c Release
./artifacts/bin/Meziantou.Framework.JavaScriptEngine.Test262/release/Meziantou.Framework.JavaScriptEngine.Test262
echo $?    # 139: the process crashed (SIGSEGV)
```

or run `./repro.sh`, which does this three times, then once with `DOTNET_TieredPGO=0`.

Expected: `Done`, exit code 0.
Actual: the process dies after about half a second, exit code 139, no output.

Tested: macOS arm64, runtime `11.0.26.50505 @Commit: 820230c386db5b0106c5430b3e5aee92a7826bf5`, crashes on every run.
The same code crashed on a Linux x64 CI runner (GitHub Actions) with the same stack.

Environment variables:

| Variable | Result |
| --- | --- |
| (none) | crash |
| `DOTNET_TieredPGO=0` | OK |
| `DOTNET_TieredCompilation=0` | OK |
| `DOTNET_TieredPGO_InstrumentOnlyHotCode=0` | crash |
| `DOTNET_TC_CallCountThreshold=1` | crash |

Workaround: add `[MethodImpl(MethodImplOptions.AggressiveOptimization)]` to `RunMode` (no crash in 4/4 runs).

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

## What was removed to get here

The first version of the repro ran the 1879 `built-ins/RegExp` tests of test262 through a test runner. It was reduced
step by step, keeping a step only when the crash still happened in 4/4 runs:

- A single trivial script (`var x = 1 + 1;`) repeated is enough: no test262 data, discovery or skip list.
- Of the runner, only the engine creation, the realm and the loop over the includes (an iterator method) are needed:
  the cancellation token, the engine options, the module loader, the agents (threads), the host objects, the
  `try`/`catch`, the strict mode, the metadata, the result handling and the `using`/`Dispose` of the engine are not.
- `JsRealm` no longer installs the built-ins (`Intrinsics.Initialize()` and `DefineGlobalBindings` are not called): a
  realm without globals is enough. The unreferenced built-ins, `Intl` and `Temporal` were deleted (138k lines of
  engine source down to about 40k, 264 files); only 4 files remain in `Builtins/` (iterators, JSON, RegExp, typed arrays) because the runtime uses them.
- The `Meziantou.Framework.Language` and `Meziantou.Framework.Language.Regex` projects are not needed (they are only
  used to parse regular expression patterns, which are stubbed), and `Meziantou.Framework.Unicode` is replaced by a stub
  of one file (`src/Unicode/Shim.cs`). The embedded data tables (time zones, CLDR) are not needed either.

What still matters (removing any of these makes the crash disappear):

- The loop must be over an iterator method (`foreach` over a `yield return` method). Over an array it does not crash.
- The loop must call `realm.Execute`. Replacing it with a cheaper call (`GC.KeepAlive`, `engine.CreateRealm()`) or
  with `JsRealm.Execute` doing less than parsing and compiling the script (no compile, no frame) does not crash.
- Stubbing the engine (same shape: `[ThreadStatic]` current engine, `using` scope struct, `Execute` returning a struct,
  non-inlined callees, slowed with `SpinWait`) does not crash. So the trigger is in how the real `JsRealm.Execute` /
  `JsEngine.CreateRealm` / `JsEngine..ctor` get inlined into `RunMode`, which I could not reproduce without the engine.

A JIT dump of the failing tier-1 compile (checked JIT, `DOTNET_JitDump`) would pinpoint the faulting block; I only
have the release JIT.

## Content

| Path | Description |
| --- | --- |
| `Repro/Program.cs` | The repro |
| `src/JavaScriptEngine/` | The JavaScript engine, copied from Meziantou.Framework and trimmed |
| `src/Unicode/` | A stub of the Unicode package that the engine references |

The code of Meziantou.Framework is under the MIT license (`LICENSE.txt`).
