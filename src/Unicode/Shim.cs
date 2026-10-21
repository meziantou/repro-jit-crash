using System.Globalization;
using System.Text;

namespace Meziantou.Framework;

public static class UnicodeProperties
{

    public static bool IsIdStart(int codePoint) => Rune.IsLetter(new Rune(codePoint));

    public static bool IsIdContinue(int codePoint) => Rune.IsLetterOrDigit(new Rune(codePoint));
}
