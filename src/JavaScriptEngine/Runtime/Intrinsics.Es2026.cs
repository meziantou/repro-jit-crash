namespace Meziantou.Framework.JavaScriptEngine;

internal sealed partial class Intrinsics
{
    /// <summary>The Array.fromAsync function (a property of %Array%).</summary>
    public JsFunction ArrayFromAsync { get; private set; } = null!;

    partial void InitializeEs2026Additions()
    {}
}
