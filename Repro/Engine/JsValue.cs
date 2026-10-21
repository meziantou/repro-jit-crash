using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>
/// A JavaScript value: <c>undefined</c>, <c>null</c>, a boolean, a number, a string, a symbol, a BigInt or an object.
/// The default value is <c>undefined</c>. Numbers are not allocated; the other values reference a <see cref="JsCell"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct JsValue : IEquatable<JsValue>
{
    // null: undefined; NumberCell.Instance: a number (in _number); the other cells are the value itself
    internal JsCell? Cell { get; }
    private readonly double _number;

    internal JsValue(JsCell? cell)
    { }

    /// <summary>Gets the type of the value.</summary>
    public JsValueKind Kind
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => throw null!;
    }

    /// <summary>Gets whether the value is a number.</summary>
    public bool IsNumber
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => throw null!;
    }

    /// <summary>Gets the number of a value that is known to be a number (no check).</summary>
    internal double NumberValue => throw null!;

    internal JsString AsJsString() => throw null!;

    /// <summary>Gets the integer of a BigInt value.</summary>
    public BigInteger AsBigInt() => throw null!;

    /// <summary>Compares two values with the SameValue algorithm (https://tc39.es/ecma262/#sec-samevalue).</summary>
    public bool Equals(JsValue other) => throw null!;
}
