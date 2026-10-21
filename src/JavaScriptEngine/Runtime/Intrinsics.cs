namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>
/// The intrinsic objects of a realm (https://tc39.es/ecma262/#sec-well-known-intrinsic-objects). The prototype objects
/// are created first, so that the built-ins can reference each other; each built-in then adds its properties
/// (Builtins/*.cs) and its global binding.
/// </summary>
internal sealed partial class Intrinsics
{
    private readonly JsObject[] _errorPrototypes = new JsObject[Enum.GetValues<ErrorType>().Length];

    public Intrinsics(JsRealm realm)
    {
        Realm = realm;

        // %Object.prototype% is an immutable prototype exotic object
        ObjectPrototype = new JsObject(prototype: null);
        ObjectPrototype.Flags |= ObjectFlags.ImmutablePrototype;

        // %Function.prototype% is a built-in function that returns undefined
        FunctionPrototype = new JsBuiltinFunction(realm, static (_, _, _, _) => JsValue.Undefined, isConstructor: false, ObjectPrototype);

        foreach (var type in Enum.GetValues<ErrorType>())
        {
            _errorPrototypes[(int)type] = new JsObject(type == ErrorType.Error ? ObjectPrototype : null);
        }

        BooleanPrototype = new JsPrimitiveObject(ObjectPrototype, ObjectClass.Boolean, JsValue.False);
        NumberPrototype = new JsPrimitiveObject(ObjectPrototype, ObjectClass.Number, 0);
        SymbolPrototype = new JsObject(ObjectPrototype);
        BigIntPrototype = new JsObject(ObjectPrototype);
        ArrayPrototype = new JsArray(ObjectPrototype);
        IteratorPrototype = new JsObject(ObjectPrototype);
        AsyncIteratorPrototype = new JsObject(ObjectPrototype);
    }

    public JsRealm Realm { get; }

    public JsObject ObjectPrototype { get; }

    public JsFunction FunctionPrototype { get; }

    public JsObject BooleanPrototype { get; }

    public JsObject NumberPrototype { get; }

    public JsObject SymbolPrototype { get; }

    public JsObject BigIntPrototype { get; }

    public JsArray ArrayPrototype { get; }

    /// <summary>%Iterator.prototype%</summary>
    public JsObject IteratorPrototype { get; }

    /// <summary>%AsyncIteratorPrototype%</summary>
    public JsObject AsyncIteratorPrototype { get; }
}
