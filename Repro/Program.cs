using Meziantou.Framework.JavaScriptEngine.Test262.Runner;

// Runs the test262 tests of built-ins/RegExp in parallel, like the xunit runner of the original project does
var tests = Test262Discovery.Discover("built-ins/RegExp").Runnable;
Console.WriteLine($"Running {tests.Count} tests");
var completed = 0;
Parallel.ForEach(tests, test =>
{
    Test262Executor.Run(test);
    Interlocked.Increment(ref completed);
});

Console.WriteLine($"Done: {completed} tests ran without crashing the process");
