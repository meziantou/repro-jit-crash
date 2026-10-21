using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>
/// A JavaScript string: a sequence of UTF-16 code units (lone surrogates allowed). A concatenation creates a rope, which
/// is flattened the first time its characters are read.
/// </summary>
[DebuggerDisplay("{ToString()}")]
internal sealed class JsString : JsCell, IEquatable<JsString>
{

    private string? _value;

    private JsString(string value)
        : base(JsValueKind.String)
    {
        _value = value;
        Length = value.Length;
    }

    public int Length { get; }

    public bool Equals([NotNullWhen(true)] JsString? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Length == other.Length && string.Equals(ToString(), other.ToString(), StringComparison.Ordinal);
    }
}
