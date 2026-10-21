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
    {
        Cell = cell;
        _number = 0;
    }

    /// <summary>Creates a number value.</summary>
    public JsValue(double value)
    {
        Cell = NumberCell.Instance;
        _number = value;
    }

    /// <summary>Gets the <c>undefined</c> value.</summary>
    public static JsValue Undefined => default;

    /// <summary>Gets the <c>false</c> value.</summary>
    public static JsValue False { get; } = new(BooleanCell.False);

    /// <summary>Gets the type of the value.</summary>
    public JsValueKind Kind
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Cell is null ? JsValueKind.Undefined : Cell.Kind;
    }

    /// <summary>Gets whether the value is a number.</summary>
    public bool IsNumber
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ReferenceEquals(Cell, NumberCell.Instance);
    }

    /// <summary>Gets the number of a value that is known to be a number (no check).</summary>
    internal double NumberValue => _number;

    /// <summary>Gets the boolean of a boolean value.</summary>
    public bool AsBoolean() => ((BooleanCell)Cell!).Value;

    /// <summary>Gets the string of a string value.</summary>
    public string AsString() => ((JsString)Cell!).ToString();

    internal JsString AsJsString() => (JsString)Cell!;

    /// <summary>Gets the symbol of a symbol value.</summary>
    public JsSymbol AsSymbol() => (JsSymbol)Cell!;

    /// <summary>Gets the integer of a BigInt value.</summary>
    public BigInteger AsBigInt() => ((JsBigInt)Cell!).Value;

    /// <summary>Gets the object of an object value.</summary>
    public JsObject AsObject() => (JsObject)Cell!;

    /// <summary>Converts a number to a value.</summary>
    public static implicit operator JsValue(int value) => new(value);

    /// <summary>Compares two values with the SameValue algorithm (https://tc39.es/ecma262/#sec-samevalue).</summary>
    public bool Equals(JsValue other) => JsOperations.SameValue(this, other);

    /// <summary>Compares the value with another <see cref="JsValue"/> with the SameValue algorithm.</summary>
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is JsValue value && Equals(value);

    /// <summary>Gets a hash code consistent with SameValue.</summary>
    public override int GetHashCode()
    {
        return Kind switch
        {
            JsValueKind.Undefined => 0,
            JsValueKind.Number => _number.GetHashCode(),
            JsValueKind.String => AsJsString().GetHashCode(),
            JsValueKind.BigInt => AsBigInt().GetHashCode(),
            _ => RuntimeHelpers.GetHashCode(Cell!),
        };
    }

    /// <summary>Returns a representation of the value for debugging; it never runs JavaScript code (use <c>ToString</c> of <see cref="JsOperations"/> for the ToString operation).</summary>
    public override string ToString()
    {
        return Kind switch
        {
            JsValueKind.Undefined => "undefined",
            JsValueKind.Null => "null",
            JsValueKind.Boolean => AsBoolean() ? "true" : "false",
            JsValueKind.Number => NumberFormatting.ToString(_number),
            JsValueKind.String => AsString(),
            JsValueKind.Symbol => AsSymbol().ToString(),
            JsValueKind.BigInt => AsBigInt().ToString(System.Globalization.CultureInfo.InvariantCulture),
            _ => Cell is HoleCell ? "<hole>" : "[object " + AsObject().ClassName + "]",
        };
    }
}
