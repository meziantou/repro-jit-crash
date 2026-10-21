using Meziantou.Framework.JavaScriptEngine.Text;

namespace Meziantou.Framework.JavaScriptEngine.Compiler;

/// <summary>
/// The compiled code of a function, a script, a module or eval code. It is immutable and does not reference any realm:
/// it can be shared by the engines of all the threads.
/// </summary>
internal sealed class FunctionTemplate
{
    public required string Name { get; init; }

    /// <summary>Gets the value of the <c>length</c> property (ExpectedArgumentCount).</summary>
    public required int Length { get; init; }

    public required FunctionTemplateFlags Flags { get; init; }

    public required int[] Code { get; init; }

    public required JsValue[] Constants { get; init; }

    public required PropertyKey[] Keys { get; init; }

    public required object[] Objects { get; init; }

    public required int RegisterCount { get; init; }

    public required int MaxStack { get; init; }

    public required ExceptionHandler[] Handlers { get; init; }

    /// <summary>Gets the pairs (pc, source offset) of the instructions, sorted by pc.</summary>
    public required int[] Positions { get; init; }

    public required SourceText Source { get; init; }

    /// <summary>Gets the start of the source text of the function (Function.prototype.toString).</summary>
    public required int SourceStart { get; init; }

    public required int SourceEnd { get; init; }
}
