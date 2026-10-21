using System.Numerics;

namespace Meziantou.Framework.JavaScriptEngine;

internal sealed class JsBigInt : JsCell
{

    public JsBigInt(BigInteger value)
        : base(JsValueKind.BigInt)
    { }

    public BigInteger Value { get; }
}
