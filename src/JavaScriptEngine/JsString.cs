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

    private static readonly JsString[] SingleCharacters = CreateSingleCharacters();

    public static readonly JsString Empty = new(string.Empty);

    private string? _value;
    private JsString? _left;
    private JsString? _right;

    private JsString(string value)
        : base(JsValueKind.String)
    {
        _value = value;
        Length = value.Length;
    }

    private JsString(JsString left, JsString right)
        : base(JsValueKind.String)
    {
        _left = left;
        _right = right;
        Length = left.Length + right.Length;
    }

    public int Length { get; }

    public static JsString Create(string value)
    {
        if (value.Length == 0)
            return Empty;

        if (value.Length == 1 && value[0] < SingleCharacters.Length)
            return SingleCharacters[value[0]];

        return new JsString(value);
    }

    public override string ToString()
    {
        if (_value is not null)
            return _value;

        Flatten();
        return _value!;
    }

    private void Flatten()
    {
        // Iterative, so that a deep rope does not overflow the stack
        var result = string.Create(Length, this, static (span, root) =>
        {
            var stack = new Stack<JsString>();
            stack.Push(root);
            var position = 0;
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                if (node._value is { } value)
                {
                    value.CopyTo(span[position..]);
                    position += value.Length;
                }
                else
                {
                    stack.Push(node._right!);
                    stack.Push(node._left!);
                }
            }
        });

        _value = result;
        _left = null;
        _right = null;
    }

    public bool Equals([NotNullWhen(true)] JsString? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Length == other.Length && string.Equals(ToString(), other.ToString(), StringComparison.Ordinal);
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is JsString other && Equals(other);

    public override int GetHashCode() => string.GetHashCode(ToString(), StringComparison.Ordinal);

    private static JsString[] CreateSingleCharacters()
    {
        var result = new JsString[128];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = new JsString(((char)i).ToString());
        }

        return result;
    }
}
