namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>
/// The host-defined operations of the engine (https://tc39.es/ecma262/#sec-host-hooks-summary). The default
/// implementations are the ones of the specification; a host (e.g. a browser) derives from this class to integrate the
/// engine with its event loop, and sets it in <see cref="JsEngineOptions.Host"/>.
/// </summary>
public class JsHost
{
}
