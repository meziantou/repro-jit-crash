namespace Meziantou.Framework.JavaScriptEngine;

internal sealed partial class Intrinsics
{
    private JsFunction? _number;
    private JsFunction? _parseInt;
    private JsFunction? _parseFloat;

    /// <summary>%Number%</summary>
    public JsFunction Number => _number!;

    /// <summary>%parseInt%, also Number.parseInt</summary>
    public JsFunction ParseInt => _parseInt!;

    /// <summary>%parseFloat%, also Number.parseFloat</summary>
    public JsFunction ParseFloat => _parseFloat!;

    partial void InitializeNumber()
    {}
}
