using Meziantou.Framework.JavaScriptEngine.Compiler;

namespace Meziantou.Framework.JavaScriptEngine.Interpreter;

/// <summary>An environment record (https://tc39.es/ecma262/#sec-environment-records) at runtime.</summary>
internal abstract class Environment(Environment? outer) : JsCell(JsValueKind.Undefined)
{
    public Environment? Outer { get; } = outer;
}

/// <summary>A binding of the declarative record of a global environment (let, const and class of the scripts).</summary>
internal sealed class GlobalLexicalBinding
{
}

/// <summary>The global environment of a realm: the global object and the lexical bindings of the scripts.</summary>
internal sealed class GlobalEnvironment(JsRealm realm) : Environment(outer: null)
{
    public JsRealm Realm { get; } = realm;

    public JsObject GlobalObject => Realm.GlobalObject;

    public Dictionary<string, GlobalLexicalBinding> LexicalBindings { get; } = new(StringComparer.Ordinal);

    /// <summary>Gets or sets the number of scripts that declared lexical bindings (the inline caches of the properties of the global object check it).</summary>
    public int LexicalVersion { get; set; }
}
