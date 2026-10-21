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

    public bool Equals(PropertyKey other)
    { throw null!; }
}
