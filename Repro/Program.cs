using Meziantou.Framework.JavaScriptEngine;

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
