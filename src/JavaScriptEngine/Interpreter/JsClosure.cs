using Meziantou.Framework.JavaScriptEngine.Compiler;

namespace Meziantou.Framework.JavaScriptEngine.Interpreter;

/// <summary>An ECMAScript function object (https://tc39.es/ecma262/#sec-ecmascript-function-objects): compiled code and its environment.</summary>
internal sealed class JsClosure : JsFunction
{
    public JsClosure(JsRealm realm, FunctionTemplate template, Environment environment, JsObject prototype, IScriptOrModule? script)
        : base(realm, prototype)
    {
        Template = template;
        Environment = environment;
        Script = script;
        if ((template.Flags & FunctionTemplateFlags.Constructor) != 0)
        {
            Flags |= ObjectFlags.Constructor;
        }

        if (template.IsClassConstructor)
        {
            Flags |= ObjectFlags.ClassConstructor;
        }
    }

    /// <summary>Gets the compiled code.</summary>
    public FunctionTemplate Template { get; }

    /// <summary>Gets the [[Environment]] of the function.</summary>
    public Environment Environment { get; internal set; }

    /// <summary>Gets the [[ScriptOrModule]] of the function.</summary>
    public IScriptOrModule? Script { get; }
}
