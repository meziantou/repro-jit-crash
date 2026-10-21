namespace Meziantou.Framework.JavaScriptEngine.Test262.Runner;

internal static class Test262Paths
{
    /// <summary>The root of the repository (the directory that contains <c>test262</c>).</summary>
    public static string Root { get; } = FindRoot();

    public static string Project { get; } = Path.Combine(Root, "Repro");

    public static string Test262 { get; } = Path.Combine(Root, "test262");

    public static string Tests { get; } = Path.Combine(Test262, "test");

    public static string Harness { get; } = Path.Combine(Test262, "harness");

    public static string SkipFile { get; } = Path.Combine(Project, "test262-skip.txt");

    public static string FeaturesFile { get; } = Path.Combine(Project, "test262-features.txt");

    private static string FindRoot()
    {
        for (var current = new DirectoryInfo(AppContext.BaseDirectory); current is not null; current = current.Parent)
        {
            if (Directory.Exists(Path.Combine(current.FullName, "test262")))
                return current.FullName;
        }

        throw new InvalidOperationException("Cannot find the test262 directory");
    }
}
