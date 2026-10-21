namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>The base class of the values that are not numbers nor <c>undefined</c>: objects, strings, symbols, BigInts, <c>null</c> and the booleans.</summary>
public abstract class JsCell
{
    private protected JsCell(JsValueKind kind)
    { }
}
