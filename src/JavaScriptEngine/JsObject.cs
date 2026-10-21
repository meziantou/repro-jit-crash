using System.Runtime.CompilerServices;

namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>
/// A JavaScript object. The internal methods (https://tc39.es/ecma262/#sec-object-internal-methods-and-internal-slots)
/// implement the ordinary object behavior; the exotic objects (arrays, proxies, arguments, typed arrays...) override them.
/// </summary>
public partial class JsObject : JsCell
{
    // Elements in which a hole is an absent property: the dense storage of the array indices (attributes: W/E/C)
    private JsValue[]? _elements;

    // The named properties (and the array indices that are not in the elements): a shape shared with other objects and the
    // values in its slots, or a property table of the object (dictionary mode, _slots is null). Null: an exotic object
    // without properties yet (its named properties may not be ordinary).
    private PropertyMap? _map;
    private JsValue[]? _slots;
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

    /// <summary>Gets a name of the kind of object, for debugging.</summary>
    internal virtual string ClassName => Class.ToString();

    /// <summary>Creates or replaces an own property (no check, no exotic behavior).</summary>
    internal void SetOwnSlot(in PropertyKey key, JsValue value, PropertyFlags flags)
    {
        if (key.IsArrayIndex)
        {
            var index = key.ArrayIndex;
            if (flags == PropertyFlags.Default && (Flags & ObjectFlags.IndexedProperties) == 0)
            {
                if (TrySetElement(index, value))
                    return;
            }
            else if (_elements is { } elements && index < (uint)elements.Length && !elements[index].IsHole)
            {
                // The property moves from the dense elements to the property table
                elements[index] = JsValue.Hole;
            }

            // The array indices that are not in the elements are in a property table
            var indexTable = GetPropertyTable();
            if ((Flags & ObjectFlags.IndexedProperties) != 0)
            {
                var existing = indexTable.Find(key);
                if (existing >= 0)
                {
                    SetTableEntry(indexTable, existing, value, flags);
                    return;
                }
            }

            Flags |= ObjectFlags.IndexedProperties;
            var indexCompactions = indexTable.Compactions;
            indexTable.Add(key, value, flags);
            OnPropertyAdded(indexTable.Compactions != indexCompactions);
            return;
        }

        if (_map is PropertyTable properties)
        {
            var position = properties.Find(key);
            if (position >= 0)
            {
                SetTableEntry(properties, position, value, flags);
            }
            else
            {
                var compactions = properties.Compactions;
                properties.Add(key, value, flags);
                OnPropertyAdded(properties.Compactions != compactions);
            }

            return;
        }

        var shape = (Shape?)_map;
        var slot = shape is null ? -1 : shape.Find(key);
        if (slot < 0)
        {
            AddProperty(key, value, flags);
        }
        else if (shape!.GetFlags(slot) == flags)
        {
            _slots![slot] = value;
        }
        else
        {
            // A reconfigured property: the object leaves the shared shapes
            var table = GetPropertyTable();
            SetTableEntry(table, table.Find(key), value, flags);
        }
    }

    private void SetTableEntry(PropertyTable table, int position, JsValue value, PropertyFlags flags)
    {
        table.GetValue(position) = value;
        ref var currentFlags = ref table.GetFlags(position);
        if (currentFlags != flags)
        {
            currentFlags = flags;
            OnStructureChanged();
        }
    }

    /// <summary>Adds a named property that does not exist: in the next shape, or in the property table.</summary>
    private void AddProperty(in PropertyKey key, JsValue value, PropertyFlags flags)
    {
        if (_map is not PropertyTable && (flags & PropertyFlags.Lazy) == 0)
        {
            var shape = (Shape?)_map ?? GetEmptyShape();
            if (shape?.AddProperty(key, flags) is { } next)
            {
                var slot = shape.Count;
                var slots = _slots;
                if (slots is null)
                {
                    _slots = slots = new JsValue[next.GetSlotsLength(0)];
                }
                else if (slot >= slots.Length)
                {
                    Array.Resize(ref _slots, next.GetSlotsLength(slots.Length));
                    slots = _slots;
                }

                slots[slot] = value;
                _map = next;
                OnPropertyAdded(positionsChanged: false);
                return;
            }
        }

        var table = GetPropertyTable();
        var tableCompactions = table.Compactions;
        table.Add(key, value, flags);
        OnPropertyAdded(table.Compactions != tableCompactions);
    }

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

    /// <summary>Gets the property table of the object: the properties of its shape move to a table of its own (dictionary mode).</summary>
    private PropertyTable GetPropertyTable(int capacity = 0)
    {
        if (_map is PropertyTable existing)
            return existing;

        // The object usually gets more properties (a table replaces the shapes of an object that has many properties)
        var shape = (Shape?)_map;
        var count = shape?.Count ?? 0;
        var table = new PropertyTable(Math.Max(Math.Max(4, count * 2), capacity));
        for (var i = 0; i < count; i++)
        {
            table.Add(shape!.GetKey(i), _slots![i], shape.GetFlags(i));
        }

        _map = table;
        _slots = null;
        OnStructureChanged();
        return table;
    }

    /// <summary>Invalidates the inline caches that depend on the object: a property was removed or reconfigured, or the properties moved.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnStructureChanged()
    {
        if ((Flags & (ObjectFlags.InlineCacheHolder | ObjectFlags.InlineCachePrototype)) != 0)
        {
            InlineCache.Invalidate();
        }
    }

    /// <summary>
    /// Invalidates the inline caches that depend on the absence of a property in the object (a prototype): a property was
    /// added. The caches that only remember the positions of its properties are still valid (unless they moved).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnPropertyAdded(bool positionsChanged)
    {
        var flags = Flags;
        if ((flags & ObjectFlags.InlineCachePrototype) != 0 || (positionsChanged && (flags & ObjectFlags.InlineCacheHolder) != 0))
        {
            InlineCache.Invalidate();
        }
    }

    /// <summary>Stores an element in the dense storage, growing it when the index is close to the end; false when it is too sparse.</summary>
    private bool TrySetElement(uint index, JsValue value)
    {
        var elements = _elements;
        if (elements is not null && index < (uint)elements.Length)
        {
            elements[index] = value;
            return true;
        }

        var currentLength = elements?.Length ?? 0;
        if (index >= (uint)Array.MaxLength / 2)
            return false;

        // Close to the end: grow. Further away (up to twice the current length): grow only when the current storage is
        // at least half full, so that a sequence of writes at exponentially increasing indices stays sparse.
        if (index > (uint)currentLength + 1024)
        {
            if (index > (uint)currentLength * 2 || !IsMostlyFilled(elements))
                return false;
        }

        var newLength = (int)Math.Min(Math.Max(Math.Max((long)currentLength * 2, (long)index + 1), 4), Array.MaxLength);
        var newElements = new JsValue[newLength];
        if (elements is not null)
        {
            Array.Copy(elements, newElements, currentLength);
        }

        newElements.AsSpan(currentLength).Fill(JsValue.Hole);
        newElements[index] = value;
        _elements = newElements;
        return true;

        static bool IsMostlyFilled(JsValue[]? elements)
        {
            if (elements is null)
                return false;

            var holes = 0;
            var maxHoles = elements.Length / 2;
            foreach (var element in elements)
            {
                if (element.IsHole && ++holes > maxHoles)
                    return false;
            }

            return true;
        }
    }

    /// <summary>Defines a built-in data property (writable, configurable, not enumerable by default) without any check.</summary>
    internal void DefineBuiltIn(in PropertyKey key, JsValue value, PropertyFlags flags = PropertyFlags.BuiltIn) => SetOwnSlot(key, value, flags);
}
