namespace Meziantou.Framework.JavaScriptEngine.Text;

/// <summary>
/// The text of a script, a module or a function, with the line map of the positions: the lines are separated by the
/// ECMAScript line terminators (LF, CR, CRLF, U+2028, U+2029) and the columns count UTF-16 code units.
/// </summary>
internal sealed class SourceText
{

    public SourceText(string text, string? url = null, int lineOffset = 0, int columnOffset = 0)
    { }

    public string Text { get; }

    /// <summary>Gets the URL (or file name) of the source, used in the error locations and the stack traces.</summary>
    public string? Url { get; }

    /// <summary>Gets the 0-based line of the first character in its document (e.g. an inline script of an HTML page).</summary>
    public int LineOffset { get; }

    /// <summary>Gets the 0-based column of the first character in its document (only the first line is shifted).</summary>
    public int ColumnOffset { get; }
}
