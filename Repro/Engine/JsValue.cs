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

    internal JsValue(JsCell? cell)
    { }

    /// <summary>Compares two values with the SameValue algorithm (https://tc39.es/ecma262/#sec-samevalue).</summary>
    public bool Equals(JsValue other) => throw null!;
}
