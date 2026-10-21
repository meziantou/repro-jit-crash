using Meziantou.Framework.JavaScriptEngine.Compiler;

namespace Meziantou.Framework.JavaScriptEngine.Interpreter;

/// <summary>
/// The execution context of a function call (or a script, eval code): its registers, its arguments, its operand stack and
/// its position. <see cref="Values"/> holds the registers, then a copy of the arguments, then the operand stack.
/// </summary>
/// <remarks>
/// The frames of the ordinary calls come from the <see cref="FramePool"/> of the engine and go back to it when the call
/// completes (<see cref="IsPooled"/>): nothing may keep a reference to them after that. The frames of the generators,
/// the async functions, the scripts, the modules and eval code are not pooled (a generator keeps its frame).
/// </remarks>
internal sealed class Frame
{
    public Frame(FunctionTemplate code, JsRealm realm, Environment environment, ReadOnlySpan<JsValue> arguments)
    {
        Values = new JsValue[code.RegisterCount + arguments.Length + code.MaxStack];
        Initialize(code, realm, environment, arguments);
    }

    /// <summary>Prepares the frame for a call (a new frame, or a frame of the pool whose values are cleared).</summary>
    internal void Initialize(FunctionTemplate code, JsRealm realm, Environment environment, ReadOnlySpan<JsValue> arguments)
    {
        Code = code;
        Realm = realm;
        Environment = environment;
        ArgumentCount = arguments.Length;
        arguments.CopyTo(Values.AsSpan(code.RegisterCount));
        Sp = code.RegisterCount + arguments.Length;
    }

    public FunctionTemplate Code { get; private set; } = null!;

    public JsRealm Realm { get; private set; } = null!;

    /// <summary>Gets or sets the function being called (null for script and eval code).</summary>
    public JsClosure? Function { get; set; }

    /// <summary>Gets or sets the this binding (a hole in a derived constructor before super()).</summary>
    public JsValue This { get; set; }

    /// <summary>Gets or sets the current lexical environment.</summary>
    public Environment Environment { get; set; } = null!;

    /// <summary>Gets the registers, the arguments, then the operand stack.</summary>
    public JsValue[] Values { get; internal set; }

    /// <summary>Gets or sets the position of the top of the operand stack in <see cref="Values"/>.</summary>
    public int Sp { get; set; }

    /// <summary>Gets or sets the position of the current instruction (while running) or of the next one (suspended, or a caller).</summary>
    public int Pc { get; set; }

    /// <summary>Gets the number of arguments (stored after the registers).</summary>
    public int ArgumentCount { get; private set; }

    public IScriptOrModule? Script { get; set; }
}
