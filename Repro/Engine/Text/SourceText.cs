namespace Meziantou.Framework.JavaScriptEngine.Text;

/// <summary>
/// The text of a script, a module or a function, with the line map of the positions: the lines are separated by the
/// ECMAScript line terminators (LF, CR, CRLF, U+2028, U+2029) and the columns count UTF-16 code units.
/// </summary>
internal sealed class SourceText
{

    public SourceText(string text, string? url = null, int lineOffset = 0, int columnOffset = 0)
    { }
}
