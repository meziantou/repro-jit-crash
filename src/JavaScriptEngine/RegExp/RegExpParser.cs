namespace Meziantou.Framework.JavaScriptEngine;

// The parser of the patterns (built on Meziantou.Framework.Language.Regex) is removed from this repro
internal static class RegExpParser
{
    public static bool TryParse(string pattern, RegExpFlags flags, out RegExpParseResult? result, out string? error)
    {
        result = null;
        error = "Not supported";
        return false;
    }

    public static bool IsBasicWordCharacter(int c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9') or '_';
}
