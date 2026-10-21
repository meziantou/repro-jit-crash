# .NET 11 RC2 JIT crash: SIGSEGV in `Compiler::fgCreateGCPoll`

On .NET 11.0.0-rc.2 the process crashes with SIGSEGV (exit code 139) while the tiered-compilation background thread
recompiles a method at tier 1 with dynamic PGO data. The crash is inside `libclrjit`, no managed frame is involved.
It does not happen on .NET 10, with `DOTNET_TieredPGO=0`, with `DOTNET_TieredCompilation=0`, or when the method has
`[MethodImpl(MethodImplOptions.AggressiveOptimization)]`.

The repro is not minimal: it runs the `built-ins/RegExp` tests of [test262](https://github.com/tc39/test262) in
parallel against a JavaScript engine written in C# (extracted from
[Meziantou.Framework](https://github.com/meziantou/Meziantou.Framework)). The method that gets compiled when the JIT
crashes is `Test262Executor.RunMode` ([Repro/Runner/Test262Executor.cs](Repro/Runner/Test262Executor.cs)).

## Reproduce

Requirements: the .NET SDK `11.0.100-rc.2.26504.105` (pinned by [global.json](global.json)). No other dependency or
network access besides NuGet restore (there is no package reference): the test262 files that are needed are vendored.

```bash
dotnet build Repro -c Release
./artifacts/bin/Meziantou.Framework.JavaScriptEngine.Test262/release/Meziantou.Framework.JavaScriptEngine.Test262
echo $?    # 139: the process crashed (SIGSEGV)
```

or run `./repro.sh`, which does this three times, then once with `DOTNET_TieredPGO=0`.

Expected: `Done: 1879 tests ran without crashing the process`, exit code 0.
Actual: the process dies after a few seconds, exit code 139, no output after `Running 1879 tests`.

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

Workaround: add `[MethodImpl(MethodImplOptions.AggressiveOptimization)]` to `Test262Executor.RunMode`.

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

`DOTNET_JitStdOutFile=<file> DOTNET_JitDisasmSummary=1` shows `RunMode` compiled twice as `Instrumented Tier0`
(IL size 726); the tier-1 compile never completes.

## Why the repro is not smaller

- A self-contained copy of the method with stubbed types did not crash: it tiered up with synthesized PGO only.
- Removing some calls of the method (`ModuleLoader`, `InstallSetTimeout`, or the agents/host setup) makes the crash
  disappear, but those calls also change the profile and the inlining decisions. Removing the code after the `try`
  block, the `catch` body or the module branch still crashes.
- A JIT dump of the failing tier-1 compile (checked JIT, `DOTNET_JitDump`) would pinpoint the faulting block; I only
  have the release JIT.

## Content

| Path | Description |
| --- | --- |
| `Repro/` | Console app: the test262 runner (`Runner/`) and `Program.cs` that runs the tests in parallel |
| `src/` | The projects the runner needs (JavaScript engine, regex syntax tree, Unicode data), copied from Meziantou.Framework |
| `test262/` | `harness/` and `test/built-ins/RegExp/` of [tc39/test262](https://github.com/tc39/test262) (commit `d53ebc8b1931d9f1541e43d59650776e4b766248`), with its license |

The code of Meziantou.Framework is under the MIT license (`LICENSE.txt`), test262 under its own license
(`test262/LICENSE`).
