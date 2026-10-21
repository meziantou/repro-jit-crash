namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>
/// The inline caches of the interpreter: each property access of the bytecode remembers where it found the property the
/// last time (<see cref="PropertyCacheEntry"/>), for the receivers that have the same <see cref="PropertyMap"/>.
/// </summary>
/// <remarks>
/// <para>
/// A cache that depends on other objects than the receiver (a property found on a prototype, or in the property table of
/// a receiver in dictionary mode) remembers the epoch: the objects it depends on are flagged
/// (<see cref="ObjectFlags.InlineCacheHolder"/>: the positions of its properties are remembered,
/// <see cref="ObjectFlags.InlineCachePrototype"/>: the absence of a property is remembered), and a change of their properties or of their prototype increments the
/// epoch, which invalidates all these caches at once. Such changes are rare once the prototypes are set up.
/// </para>
/// <para>
/// The caches are in the <see cref="Compiler.FunctionTemplate"/>, which the engines of several threads can share: an entry
/// is immutable and replaced as a whole, and it only matches the objects of the engine that created it (the shapes with
/// properties belong to an engine, the static empty shapes are checked with the prototype).
/// </para>
/// </remarks>
internal static class InlineCache
{
    private static int s_epoch;

    /// <summary>Invalidates the caches that depend on other objects than the receiver.</summary>
    public static void Invalidate() => Interlocked.Increment(ref s_epoch);
}

/// <summary>An entry of an inline cache (immutable once stored).</summary>
internal sealed class PropertyCacheEntry
{
}
