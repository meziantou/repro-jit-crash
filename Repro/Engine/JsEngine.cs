using System.Runtime.CompilerServices;

namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>
/// An ECMAScript agent (https://tc39.es/ecma262/#sec-agents): it owns the realms, the job queue and the execution limits
/// of the scripts it runs. An engine is used by one thread at a time.
/// </summary>
public sealed partial class JsEngine : IDisposable
{
    [ThreadStatic]
    private static JsEngine? s_current;

    private readonly List<JsRealm> _realms = [];

    /// <summary>Initializes a new instance of the <see cref="JsEngine"/> class.</summary>
    public JsEngine(JsEngineOptions? options = null)
    {
        Options = options ?? new JsEngineOptions();
        HostHooks = Options.Host ?? JsHost.Default;
    }

    /// <summary>Gets the options of the engine.</summary>
    public JsEngineOptions Options { get; }

    /// <summary>Creates a realm with its intrinsics and an ordinary global object.</summary>
    public JsRealm CreateRealm()
    {
        using var scope = Enter();
        var realm = new JsRealm(this);
        _realms.Add(realm);
        return realm;
    }

    /// <summary>
    /// Makes the engine the engine of the current thread until the returned scope is disposed. The methods of the objects
    /// (<see cref="JsObject.GetProperty(JsPropertyKey)"/>...) need it when they are not called by the engine (a callback).
    /// </summary>
    public JsEngineScope Enter()
    {
        var previous = s_current;
        s_current = this;
        return new JsEngineScope(previous);
    }

    /// <summary>Releases the resources of the engine.</summary>
    public void Dispose()
    {
    }

    internal static void RestoreCurrent(JsEngine? previous) => s_current = previous;
}
