using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>The abstract operations of ECMAScript (https://tc39.es/ecma262/#sec-abstract-operations).</summary>
internal static class JsOperations
{
    // ------------------------------------------------------------------------------------------------------------------
    // Testing and comparison (https://tc39.es/ecma262/#sec-testing-and-comparison-operations)
    // ------------------------------------------------------------------------------------------------------------------

    /// <summary>SameValue(x, y)</summary>
    internal static bool SameValue(JsValue x, JsValue y)
    { throw null!; }

    private static bool SameValueNonNumber(JsValue x, JsValue y)
    { throw null!; }
}
