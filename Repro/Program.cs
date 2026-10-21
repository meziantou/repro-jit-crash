using Meziantou.Framework.JavaScriptEngine.Compiler;
using Meziantou.Framework.JavaScriptEngine.Text;
using Meziantou.Framework.JavaScriptEngine;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

var repeat = args.Length > 0 ? int.Parse(args[0]) : 500_000;
Parallel.For(0, repeat, _ => RunMode("var x = 1 + 1;"));
Console.WriteLine("Done");

static string? RunMode(string source)
{
    var engine = new JsEngine(new JsEngineOptions());
    var realm = engine.CreateRealm();
    foreach (var include in GetIncludes())
    {
        realm.Execute("var harness = 1;", include);
    }

    return null;
}

static IEnumerable<string> GetIncludes()
{
    yield return "assert.js";
    yield return "sta.js";
}

namespace Meziantou.Framework.JavaScriptEngine
{
    public readonly struct JsEngineScope : IDisposable, IEquatable<JsEngineScope>
    {
        private readonly JsEngine? _previous;

        internal JsEngineScope(JsEngine? previous) => _previous = previous;
        public void Dispose() => JsEngine.RestoreCurrent(_previous);
        public bool Equals(JsEngineScope other) => throw null!;
    }
}

namespace Meziantou.Framework.JavaScriptEngine.Compiler
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto)]
    internal readonly record struct ExceptionHandler(int Start, int End, int Target, int StackDepth, int EnvironmentRegister);
}

namespace Meziantou.Framework.JavaScriptEngine.Compiler
{
    internal sealed class FunctionTemplate
    {
        public required string Name { get; init; }
        public required int Length { get; init; }

        public required FunctionTemplateFlags Flags { get; init; }

        public required int[] Code { get; init; }

        public required JsValue[] Constants { get; init; }

        public required PropertyKey[] Keys { get; init; }

        public required object[] Objects { get; init; }

        public required int RegisterCount { get; init; }

        public required int MaxStack { get; init; }

        public required ExceptionHandler[] Handlers { get; init; }
        public required int[] Positions { get; init; }

        public required SourceText Source { get; init; }
        public required int SourceStart { get; init; }

        public required int SourceEnd { get; init; }
    }
}

namespace Meziantou.Framework.JavaScriptEngine.Compiler
{
    [Flags]
    internal enum FunctionTemplateFlags
    {
        None = 0,
        Strict = 1 << 0,
        Arrow = 1 << 1,
        Async = 1 << 2,
        Generator = 1 << 3,
        Constructor = 1 << 4,
        ClassConstructor = 1 << 5,
        DerivedConstructor = 1 << 6,
        Method = 1 << 7,
        ScriptCode = 1 << 8,
        EvalCode = 1 << 9,
        ModuleCode = 1 << 10,
        ClassFieldInitializer = 1 << 11,
        Anonymous = 1 << 12,
        DefaultConstructor = 1 << 13,
    }
}

namespace Meziantou.Framework.JavaScriptEngine.Interpreter
{
    internal abstract class Environment(Environment? outer) : JsCell(JsValueKind.Undefined)
    {
    }
    internal sealed class GlobalEnvironment(JsRealm realm) : Environment(outer: null)
    {
    }
}

namespace Meziantou.Framework.JavaScriptEngine.Interpreter
{
    internal sealed class Frame
    {
        public Frame(FunctionTemplate code, ReadOnlySpan<JsValue> arguments)
        {
            Values = new JsValue[code.RegisterCount + arguments.Length + code.MaxStack];
            arguments.CopyTo(Values.AsSpan(code.RegisterCount));
        }
        public JsValue[] Values { get; internal set; }
    }
}

namespace Meziantou.Framework.JavaScriptEngine
{
    public abstract class JsCell
    {
        private protected JsCell(JsValueKind kind)
        { }
    }
}

namespace Meziantou.Framework.JavaScriptEngine
{
    public sealed partial class JsEngine : IDisposable
    {
        [ThreadStatic]
        private static JsEngine? s_current;

        private readonly List<JsRealm> _realms = [];
        public JsEngine(JsEngineOptions? options = null)
        { }
        public JsRealm CreateRealm()
        {
            using var scope = Enter();
            var realm = new JsRealm(this);
            _realms.Add(realm);
            return realm;
        }
        public JsEngineScope Enter()
        {
            var previous = s_current;
            s_current = this;
            return new JsEngineScope(previous);
        }
        public void Dispose()
        {
        }

        internal static void RestoreCurrent(JsEngine? previous) => s_current = previous;
    }
}

namespace Meziantou.Framework.JavaScriptEngine
{
    public sealed class JsEngineOptions
    {
    }
}

namespace Meziantou.Framework.JavaScriptEngine
{
    public partial class JsObject : JsCell
    {

        internal JsObject(JsObject? prototype, ObjectClass objectClass = ObjectClass.Object)
            : base(JsValueKind.Object)
        { }
    }
}

namespace Meziantou.Framework.JavaScriptEngine
{
    public sealed partial class JsRealm
    {
        internal JsRealm(JsEngine engine)
        {
            Engine = engine;
            Intrinsics = null!;
            GlobalObject = new JsObject(prototype: null);
            GlobalThis = new JsValue(GlobalObject);
            GlobalEnvironment = null!;
        }

        public JsEngine Engine { get; }
        public JsObject GlobalObject { get; }
        public JsValue GlobalThis { get; }

        internal Intrinsics Intrinsics { get; }
        internal Interpreter.GlobalEnvironment GlobalEnvironment { get; }
        public JsValue Execute(string code, string? url = null)
        {
            using var scope = Engine.Enter();
            var source = new Text.SourceText(code, url);
            return Run(source, Compile(source), hostDefined: null, mutedErrors: false);
        }

        private JsValue Run(Text.SourceText source, Compiler.FunctionTemplate template, object? hostDefined, bool mutedErrors)
        {
            var frame = new Interpreter.Frame(template, []);
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
    }
}

namespace Meziantou.Framework.JavaScriptEngine
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct JsValue : IEquatable<JsValue>
    {
        // null: undefined; NumberCell.Instance: a number (in _number); the other cells are the value itself
        internal JsCell? Cell { get; }

        internal JsValue(JsCell? cell)
        { }
        public bool Equals(JsValue other) => throw null!;
    }
}

namespace Meziantou.Framework.JavaScriptEngine
{
    public enum JsValueKind
    {
        Undefined,
        Null,
        Boolean,
        Number,
        String,
        Symbol,
        BigInt,
        Object,
    }
}

namespace Meziantou.Framework.JavaScriptEngine
{
    [DebuggerDisplay("{ToString()}")]
    internal readonly struct PropertyKey : IEquatable<PropertyKey>
    {

        public bool Equals(PropertyKey other)
        { throw null!; }
    }
}

namespace Meziantou.Framework.JavaScriptEngine
{
    internal sealed partial class Intrinsics
    {
    }
}

namespace Meziantou.Framework.JavaScriptEngine
{
    internal enum ObjectClass : byte
    {
        Object,
        Array,
        Function,
        Error,
        Boolean,
        Number,
        String,
        Symbol,
        BigInt,
        Date,
        RegExp,
        Arguments,
        Map,
        Set,
        WeakMap,
        WeakSet,
        WeakRef,
        FinalizationRegistry,
        ArrayBuffer,
        SharedArrayBuffer,
        DataView,
        TypedArray,
        Promise,
        Proxy,
        Generator,
        AsyncGenerator,
        AsyncFromSyncIterator,
        ModuleNamespace,
        Iterator,
        DisposableStack,
        AsyncDisposableStack,
        Temporal,
        Intl,
        Host,
    }
}

namespace Meziantou.Framework.JavaScriptEngine.Text
{
    internal sealed class SourceText
    {

        public SourceText(string text, string? url = null, int lineOffset = 0, int columnOffset = 0)
        { }
    }
}
