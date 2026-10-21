using System.Runtime.CompilerServices;

namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>
/// The layout of the named properties of an object (a hidden class): the keys and the attributes, in insertion order. The
/// values are in the slots of the object, at the position of their key. The objects that get the same properties in the
/// same order share their shape, so the inline caches of the interpreter can remember where a property is by remembering
/// the shape (<see cref="InlineCache"/>).
/// </summary>
/// <remarks>
/// <para>
/// A shape is immutable: adding a property moves the object to a child shape (a transition, cached in the parent). The
/// objects whose properties are deleted or reconfigured, or that have many properties, use a <see cref="PropertyTable"/>
/// of their own instead (the dictionary mode).
/// </para>
/// <para>
/// The empty shapes (<see cref="EmptyOrdinary"/>...) are static: they have no transitions. The shapes with properties are
/// created per engine (<see cref="ShapeRoots"/>), so the transition trees are not shared between threads and are freed
/// with their engine.
/// </para>
/// </remarks>
internal sealed class Shape : PropertyMap
{

    /// <summary>The shape of the ordinary objects without named properties.</summary>
    public static readonly Shape EmptyOrdinary = new(ShapeKind.Ordinary);

    private Shape(ShapeKind kind)
    {
        Kind = kind;
    }

    /// <summary>Gets the kind of the objects that use the shape (the root of the tree).</summary>
    public ShapeKind Kind { get; }
}

/// <summary>The kinds of objects that can share shapes: the objects whose named properties are all ordinary.</summary>
internal enum ShapeKind : byte
{
    /// <summary>The ordinary objects (and the exotic objects whose named properties are ordinary, e.g. functions).</summary>
    Ordinary,

    /// <summary>The arrays: length is exotic.</summary>
    Array,

    /// <summary>The String objects and the arguments objects: the array indices are exotic.</summary>
    IndexedExotic,
}

/// <summary>The storage of the named properties of an object: a shared <see cref="Shape"/>, or a <see cref="PropertyTable"/> of its own (dictionary mode).</summary>
internal abstract class PropertyMap
{
}
