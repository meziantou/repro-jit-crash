namespace Meziantou.Framework.JavaScriptEngine.Test262.Runner;

/// <summary>The frontmatter of a test262 file (https://github.com/tc39/test262/blob/main/INTERPRETING.md#metadata).</summary>
internal sealed class Test262Metadata
{
    public IReadOnlyList<string> Includes { get; private init; } = [];

    public IReadOnlyList<string> Flags { get; private init; } = [];

    public IReadOnlyList<string> Features { get; private init; } = [];

    public IReadOnlyList<string> Locales { get; private init; } = [];

    /// <summary>The phase of the expected error (<c>parse</c>, <c>resolution</c> or <c>runtime</c>), or null for a positive test.</summary>
    public string? NegativePhase { get; private init; }

    /// <summary>The name of the constructor of the expected error (e.g. <c>SyntaxError</c>).</summary>
    public string? NegativeType { get; private init; }

    public bool HasFlag(string flag) => Flags.Contains(flag, StringComparer.Ordinal);

    public bool IsModule => HasFlag("module");

    public bool IsRaw => HasFlag("raw");

    public bool IsAsync => HasFlag("async");

    public static Test262Metadata Parse(string source)
    {
        var start = source.IndexOf("/*---", StringComparison.Ordinal);
        if (start < 0)
            return new Test262Metadata();

        var end = source.IndexOf("---*/", start, StringComparison.Ordinal);
        if (end < 0)
            throw new FormatException("The frontmatter is not terminated");

        var lines = source[(start + 5)..end].Split('\n').Select(line => line.TrimEnd('\r')).ToList();
        var lists = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var negative = new Dictionary<string, string>(StringComparer.Ordinal);
        string? key = null;
        foreach (var line in lines)
        {
            if (line.Trim().Length == 0)
                continue;

            if (!char.IsWhiteSpace(line[0]))
            {
                var colon = line.IndexOf(':', StringComparison.Ordinal);
                if (colon < 0)
                    continue;

                key = line[..colon].Trim();
                var value = line[(colon + 1)..].Trim();
                if (value.StartsWith('[') && value.EndsWith(']'))
                {
                    lists[key] = [.. value[1..^1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];
                }
                else if (value.Length > 0 && value is not ">" and not "|" and not ">-" and not "|-")
                {
                    lists[key] = [value];
                }
            }
            else if (key is not null)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("- ", StringComparison.Ordinal))
                {
                    if (!lists.TryGetValue(key, out var list))
                        lists[key] = list = [];

                    list.Add(trimmed[2..].Trim());
                }
                else if (key == "negative" && trimmed.IndexOf(':', StringComparison.Ordinal) is var colon and > 0)
                {
                    negative[trimmed[..colon].Trim()] = trimmed[(colon + 1)..].Trim();
                }
            }
        }

        return new Test262Metadata
        {
            Includes = GetList(lists, "includes"),
            Flags = GetList(lists, "flags"),
            Features = GetList(lists, "features"),
            Locales = GetList(lists, "locale"),
            NegativePhase = negative.GetValueOrDefault("phase"),
            NegativeType = negative.GetValueOrDefault("type"),
        };
    }

    private static string[] GetList(Dictionary<string, List<string>> lists, string key) => lists.TryGetValue(key, out var list) ? [.. list] : [];
}
