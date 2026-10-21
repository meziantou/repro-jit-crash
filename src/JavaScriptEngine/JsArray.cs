namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>An Array exotic object (https://tc39.es/ecma262/#sec-array-exotic-objects): <c>length</c> follows the indices.</summary>
public sealed class JsArray : JsObject
{
    private uint _length;

    internal JsArray(JsObject? prototype, uint length = 0)
        : base(prototype, ObjectClass.Array)
    {
        _length = length;
        Flags |= ObjectFlags.Exotic;
    }
}
