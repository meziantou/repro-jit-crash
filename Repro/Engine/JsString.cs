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

    private JsString(string value)
        : base(JsValueKind.String)
    { }

    public bool Equals([NotNullWhen(true)] JsString? other)
    { throw null!; }
}
