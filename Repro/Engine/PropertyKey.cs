using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>
/// A property key: a string or a symbol (https://tc39.es/ecma262/#sec-object-type). The array indices (the canonical
/// numeric strings of the integers 0 to 2^32-2) are kept as numbers.
/// </summary>
[DebuggerDisplay("{ToString()}")]
internal readonly struct PropertyKey : IEquatable<PropertyKey>
{
    private const uint NotAnIndex = uint.MaxValue;

    // A string, a JsSymbol, or null for an array index
    private readonly object? _value;
    private readonly uint _index;

    public bool Equals(PropertyKey other)
    {
        if (_index != NotAnIndex || other._index != NotAnIndex)
            return _index == other._index;

        if (_value is string a)
            return other._value is string b && string.Equals(a, b, StringComparison.Ordinal);

        return ReferenceEquals(_value, other._value);
    }
}
