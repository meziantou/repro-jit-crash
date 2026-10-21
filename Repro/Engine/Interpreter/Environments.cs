using Meziantou.Framework.JavaScriptEngine.Compiler;

namespace Meziantou.Framework.JavaScriptEngine.Interpreter;

/// <summary>An environment record (https://tc39.es/ecma262/#sec-environment-records) at runtime.</summary>
internal abstract class Environment(Environment? outer) : JsCell(JsValueKind.Undefined)
{
}

/// <summary>The global environment of a realm: the global object and the lexical bindings of the scripts.</summary>
internal sealed class GlobalEnvironment(JsRealm realm) : Environment(outer: null)
{
}
