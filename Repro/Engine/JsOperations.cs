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
    {
        if (x.IsNumber)
        {
            if (!y.IsNumber)
                return false;

            var a = x.NumberValue;
            var b = y.NumberValue;
            if (double.IsNaN(a))
                return double.IsNaN(b);

            return a.Equals(b) && double.IsNegative(a) == double.IsNegative(b);
        }

        return SameValueNonNumber(x, y);
    }

    private static bool SameValueNonNumber(JsValue x, JsValue y)
    {
        if (ReferenceEquals(x.Cell, y.Cell))
            return true;

        if (x.Kind != y.Kind)
            return false;

        return x.Kind switch
        {
            JsValueKind.String => x.AsJsString().Equals(y.AsJsString()),
            JsValueKind.BigInt => x.AsBigInt() == y.AsBigInt(),
            _ => false,
        };
    }
}
