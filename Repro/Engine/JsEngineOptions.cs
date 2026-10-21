namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>The options of a <see cref="JsEngine"/>: the limits of the scripts and the host services.</summary>
public sealed class JsEngineOptions
{

    /// <summary>Gets or sets the host-defined operations (promise rejection tracking, job callbacks, compilation of strings...), or null for the default ones.</summary>
    public JsHost? Host { get; set; }
}
