namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>A JavaScript function: an object with a [[Call]] internal method, and possibly a [[Construct]] internal method.</summary>
public abstract class JsFunction : JsObject
{
    private protected JsFunction(JsRealm realm, JsObject? prototype)
        : base(prototype, ObjectClass.Function)
    {
        Realm = realm;
        Flags |= ObjectFlags.Callable;
    }

    /// <summary>Gets the realm of the function ([[Realm]]).</summary>
    public JsRealm Realm { get; }

    /// <summary>Gets the name given by SetFunctionName ([[InitialName]]), used by <c>Function.prototype.toString</c>.</summary>
    internal string? InitialName { get; private set; }
}
