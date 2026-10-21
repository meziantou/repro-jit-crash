using System.Runtime.CompilerServices;

namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>
/// A JavaScript object. The internal methods (https://tc39.es/ecma262/#sec-object-internal-methods-and-internal-slots)
/// implement the ordinary object behavior; the exotic objects (arrays, proxies, arguments, typed arrays...) override them.
/// </summary>
public partial class JsObject : JsCell
{

    // The named properties (and the array indices that are not in the elements): a shape shared with other objects and the
    // values in its slots, or a property table of the object (dictionary mode, _slots is null). Null: an exotic object
    // without properties yet (its named properties may not be ordinary).
    private PropertyMap? _map;
    private ObjectFlags _flags;
    private JsObject? _prototype;

    internal JsObject(JsObject? prototype, ObjectClass objectClass = ObjectClass.Object)
        : base(JsValueKind.Object)
    {
        _prototype = prototype;
        Class = objectClass;
        _flags = ObjectFlags.Extensible;
        _map = Shape.EmptyOrdinary;
    }

    internal ObjectFlags Flags
    {
        get => _flags;
        set
        {
            _flags = value;

            // An exotic object does not share the shapes of the ordinary objects: the inline caches would skip its internal methods
            if ((value & ObjectFlags.Exotic) != 0 && ReferenceEquals(_map, Shape.EmptyOrdinary))
            {
                _map = GetEmptyShape();
            }
        }
    }

    internal ObjectClass Class { get; set; }

    internal bool IsExotic => (Flags & ObjectFlags.Exotic) != 0;

    /// <summary>Gets the empty shape of the kind of the object, or null when its named properties are exotic (it uses a property table).</summary>
    private Shape? GetEmptyShape()
    {
        if (!IsExotic)
            return Shape.EmptyOrdinary;

        return this switch
        {
            JsArray => Shape.EmptyArray,
            _ => null,
        };
    }
}
