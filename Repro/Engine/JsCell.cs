namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>The base class of the values that are not numbers nor <c>undefined</c>: objects, strings, symbols, BigInts, <c>null</c> and the booleans.</summary>
public abstract class JsCell
{
    private protected JsCell(JsValueKind kind)
    {
        Kind = kind;
    }

    internal JsValueKind Kind { get; }
}

/// <summary>The cell of the number values (the number itself is stored in the <see cref="JsValue"/>).</summary>
internal sealed class NumberCell : JsCell
{
    public static readonly NumberCell Instance = new();

    private NumberCell()
        : base(JsValueKind.Number)
    {
    }
}
