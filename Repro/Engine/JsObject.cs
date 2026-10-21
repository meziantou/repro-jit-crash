using System.Runtime.CompilerServices;

namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>
/// A JavaScript object. The internal methods (https://tc39.es/ecma262/#sec-object-internal-methods-and-internal-slots)
/// implement the ordinary object behavior; the exotic objects (arrays, proxies, arguments, typed arrays...) override them.
/// </summary>
public partial class JsObject : JsCell
{

    internal JsObject(JsObject? prototype, ObjectClass objectClass = ObjectClass.Object)
        : base(JsValueKind.Object)
    { }
}
