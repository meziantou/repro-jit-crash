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
    public Frame(FunctionTemplate code, ReadOnlySpan<JsValue> arguments)
    {
        Values = new JsValue[code.RegisterCount + arguments.Length + code.MaxStack];
        arguments.CopyTo(Values.AsSpan(code.RegisterCount));
    }



    /// <summary>Gets or sets the current lexical environment.</summary>

    /// <summary>Gets the registers, the arguments, then the operand stack.</summary>
    public JsValue[] Values { get; internal set; }

    /// <summary>Gets or sets the position of the top of the operand stack in <see cref="Values"/>.</summary>

    /// <summary>Gets the number of arguments (stored after the registers).</summary>
}
