using System.Diagnostics;
using System.Runtime.CompilerServices;
using Meziantou.Framework.JavaScriptEngine.Interpreter;

namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>The execution context stack and the limits of the running scripts.</summary>
public sealed partial class JsEngine
{

    private readonly List<Frame> _frames = [];

    /// <summary>Gets the frames of the running code, the innermost last.</summary>
    internal IReadOnlyList<Frame> Frames => _frames;
}
