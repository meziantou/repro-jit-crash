using System.Globalization;
using System.Text;

namespace Meziantou.Framework;

// A minimal replacement of the Meziantou.Framework.Unicode package: the engine of this repro does not need real data.
public enum UnicodeScript { Unknown, Hebrew }

public enum UnicodeWordBreak : byte { Other, SingleQuote, MidNumLet, MidLetter }

public enum UnicodeWordKind { None }

public static class UnicodeProperties
{
    public static int MaxCaseMappingLength => 3;

    public static byte GetCanonicalCombiningClass(int codePoint) => 0;

    public static UnicodeCategory GetGeneralCategory(int codePoint) => CharUnicodeInfo.GetUnicodeCategory(codePoint);

    public static int ToSimpleCaseFold(int codePoint) => codePoint;

    public static int GetUppercaseMapping(int codePoint, Span<int> destination)
    {
        destination[0] = codePoint;
        return 1;
    }

    public static int GetLowercaseMapping(int codePoint, Span<int> destination)
    {
        destination[0] = codePoint;
        return 1;
    }

    public static bool IsIdStart(int codePoint) => Rune.IsLetter(new Rune(codePoint));

    public static bool IsIdContinue(int codePoint) => Rune.IsLetterOrDigit(new Rune(codePoint));

    public static UnicodeWordBreak GetWordBreak(int codePoint) => UnicodeWordBreak.Other;
}

public static class UnicodeScripts
{
    public static UnicodeScript GetScript(int codePoint) => UnicodeScript.Unknown;

    public static UnicodeScript GetScript(Rune rune) => UnicodeScript.Unknown;

    public static string GetIso15924Code(UnicodeScript script) => "Zzzz";
}

public static class UnicodeNormalization
{
    public static int MaxCanonicalDecompositionLength => 4;

    public static int GetCanonicalDecomposition(int codePoint, Span<int> destination) => 0;

    public static string Normalize(string value, NormalizationForm form = NormalizationForm.FormC) => value;

    public static bool IsNormalized(ReadOnlySpan<char> value, NormalizationForm form = NormalizationForm.FormC) => true;

    public static int GetNormalizedLength(ReadOnlySpan<char> source, NormalizationForm form = NormalizationForm.FormC) => source.Length;
}

public static class UnicodeCaseMapping
{
    public static string ToUpper(string value, string? language = null) => value.ToUpperInvariant();

    public static string ToLower(string value, string? language = null) => value.ToLowerInvariant();
}

public static class Unicode
{
    public static IEnumerable<Range> EnumerateGraphemeClusters(ReadOnlySpan<char> text) => [new Range(0, text.Length)];

    public static IEnumerable<Range> EnumerateSentenceBoundaries(ReadOnlySpan<char> text) => [new Range(0, text.Length)];

    public static UnicodeWordEnumerator EnumerateWordBoundaries(ReadOnlySpan<char> text) => new(text.Length);
}

public struct UnicodeWordEnumerator(int length)
{
    private bool _done;

    public Range Current { get; private set; }

    public UnicodeWordKind CurrentKind => UnicodeWordKind.None;

    public bool MoveNext()
    {
        if (_done)
            return false;

        _done = true;
        Current = new Range(0, length);
        return true;
    }
}
