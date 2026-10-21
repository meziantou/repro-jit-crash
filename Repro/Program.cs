var repeat = args.Length > 0 ? int.Parse(args[0]) : 500_000;
Parallel.For(0, repeat, _ => RunMode("var x = 1 + 1;"));
Console.WriteLine("Done");

static string? RunMode(string source)
{
    var engine = new JsEngine();
    var realm = engine.CreateRealm();
    foreach (var include in GetIncludes())
    {
        realm.Execute(include);
    }

    return null;
}

static IEnumerable<string> GetIncludes()
{
    yield return "assert.js";
    yield return "sta.js";
}

public sealed class JsEngine
{
    [ThreadStatic]
    private static JsEngine? s_current;
    private readonly List<JsRealm> _realms = [];

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

    internal static void RestoreCurrent(JsEngine? previous) => s_current = previous;
}

public readonly struct JsEngineScope : IDisposable
{
    private readonly JsEngine? _previous;

    internal JsEngineScope(JsEngine? previous) => _previous = previous;

    public void Dispose() => JsEngine.RestoreCurrent(_previous);
}

public sealed class JsRealm(JsEngine engine)
{
    public JsEngine Engine { get; } = engine;

    public JsValue Execute(string code)
    {
        using var scope = Engine.Enter();
        var source = code;
        return Run(source, Compile(source));
    }

    private JsValue Run(string source, FunctionTemplate template)
    {
        var frame = new Frame(template, []);
        GC.KeepAlive(frame);
        return default;
    }

    private FunctionTemplate Compile(string source)
    {
        return new FunctionTemplate
        {
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

internal sealed class FunctionTemplate
{
    public required int[] Code { get; init; }
    public required JsValue[] Constants { get; init; }
    public required int[] Keys { get; init; }
    public required int[] Objects { get; init; }
    public required int[] Handlers { get; init; }
    public required int[] Positions { get; init; }
    public required int SourceStart { get; init; }
    public required int SourceEnd { get; init; }
    public required int RegisterCount { get; init; }

    public required int MaxStack { get; init; }

    public required string Source { get; init; }
}

internal sealed class Frame
{
    public Frame(FunctionTemplate code, ReadOnlySpan<JsValue> arguments)
    {
        Values = new JsValue[code.RegisterCount + arguments.Length + code.MaxStack];
        arguments.CopyTo(Values.AsSpan(code.RegisterCount));
    }

    public JsValue[] Values;
}

public readonly struct JsValue
{
    internal object? Cell { get; }

    internal JsValue(object? cell)
    {
    }
}

