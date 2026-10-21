using Meziantou.Framework.JavaScriptEngine.Ast;
using Meziantou.Framework.JavaScriptEngine.Text;

namespace Meziantou.Framework.JavaScriptEngine.Parsing;

/// <summary>Parses ECMAScript source text into a syntax tree, with all the early errors of the specification.</summary>
internal static class JsParser
{
    /// <summary>Parses a script (or the code of an eval) and reports the early errors.</summary>
    /// <exception cref="JsSyntaxErrorException">The text is not a valid script.</exception>
    public static Program ParseScript(SourceText source, ParserOptions options) => Parser.ParseScript(source, options);
}
