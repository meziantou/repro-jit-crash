namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>A built-in function object (https://tc39.es/ecma262/#sec-built-in-function-objects) implemented in .NET.</summary>
internal sealed class JsBuiltinFunction : JsFunction
{
    private readonly BuiltinFunctionCallback _callback;

    public JsBuiltinFunction(JsRealm realm, BuiltinFunctionCallback callback, bool isConstructor, JsObject? prototype)
        : base(realm, prototype)
    {
        _callback = callback;
        if (isConstructor)
        {
            Flags |= ObjectFlags.Constructor;
        }
    }
}
