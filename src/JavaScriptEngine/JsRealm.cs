namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>A realm (https://tc39.es/ecma262/#sec-code-realms): a global object, its global environment and the intrinsics.</summary>
public sealed partial class JsRealm
{
    internal JsRealm(JsEngine engine)
    {
        Engine = engine;
        var previousRealm = engine.CurrentRealm;
        engine.CurrentRealm = this;
        try
        {
            Intrinsics = new Intrinsics(this);
            GlobalObject = new JsObject(Intrinsics.ObjectPrototype);
            GlobalThis = new JsValue(GlobalObject);
            GlobalEnvironment = new Interpreter.GlobalEnvironment(this);
        }
        finally
        {
            engine.CurrentRealm = previousRealm ?? this;
        }
    }

    /// <summary>Gets the engine (the agent) of the realm.</summary>
    public JsEngine Engine { get; }

    /// <summary>Gets the global object ([[GlobalObject]]).</summary>
    public JsObject GlobalObject { get; }

    /// <summary>Gets the <c>this</c> value of the global code ([[GlobalThisValue]] of the global environment).</summary>
    public JsValue GlobalThis { get; }

    internal Intrinsics Intrinsics { get; }

    /// <summary>Gets the global environment of the realm.</summary>
    internal Interpreter.GlobalEnvironment GlobalEnvironment { get; }

    /// <summary>Runs a script in this realm (ScriptEvaluation) and returns its completion value.</summary>
    /// <param name="code">The source text of the script.</param>
    /// <param name="url">The URL (or file name) of the script, used in the stack traces and the error locations.</param>
    /// <exception cref="JsException">The script threw an exception (a SyntaxError for an invalid script).</exception>
    public JsValue Execute(string code, string? url = null)
    {
        using var scope = Engine.Enter();
        var source = new Text.SourceText(code, url);
        return Run(source, Compile(source), hostDefined: null, mutedErrors: false);
    }

    private JsValue Run(Text.SourceText source, Compiler.FunctionTemplate template, object? hostDefined, bool mutedErrors)
    {
        var script = new JsScript(this, source, hostDefined, mutedErrors);
        var frame = new Interpreter.Frame(template, this, GlobalEnvironment, [])
        {
            Script = script,
            This = GlobalThis,
        };

        GC.KeepAlive(frame);
        return default;
    }

    private Compiler.FunctionTemplate Compile(Text.SourceText source)
    {
        return new Compiler.FunctionTemplate
        {
            Name = "",
            Length = 0,
            Flags = default,
            Code = [],
            Constants = [],
            Keys = [],
            Objects = [],
            RegisterCount = 1,
            MaxStack = 1,
            Handlers = [],
            Positions = [],
            Source = source,
            SourceStart = 0,
            SourceEnd = 0,
        };
    }

    /// <summary>Creates a native error object of this realm.</summary>
    internal JsObject CreateError(ErrorType type, string message)
    {
        var error = new JsErrorObject(Intrinsics.GetErrorPrototype(type));
        error.DefineBuiltIn("message", message);
        error.CaptureStackTrace(Engine);
        return error;
    }
}
