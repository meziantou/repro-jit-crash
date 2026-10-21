using Meziantou.Framework.JavaScriptEngine.Interpreter;

namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>The generators, the async functions, the async generators and eval.</summary>
internal sealed partial class Intrinsics
{
    private JsObject? _generatorFunctionPrototype;
    private JsObject? _generatorPrototype;
    private JsObject? _asyncFunctionPrototype;
    private JsObject? _asyncGeneratorFunctionPrototype;
    private JsObject? _asyncGeneratorPrototype;
    private JsObject? _asyncFromSyncIteratorPrototype;
    private JsFunction? _eval;

    /// <summary>%GeneratorFunction.prototype%: the prototype of the generator functions.</summary>
    public JsObject GeneratorFunctionPrototype => _generatorFunctionPrototype!;

    /// <summary>%GeneratorPrototype% (%GeneratorFunction.prototype.prototype%).</summary>
    public JsObject GeneratorPrototype => _generatorPrototype!;

    /// <summary>%AsyncFunction.prototype%</summary>
    public JsObject AsyncFunctionPrototype => _asyncFunctionPrototype!;

    /// <summary>%AsyncGeneratorFunction.prototype%</summary>
    public JsObject AsyncGeneratorFunctionPrototype => _asyncGeneratorFunctionPrototype!;

    /// <summary>%AsyncGeneratorPrototype%</summary>
    public JsObject AsyncGeneratorPrototype => _asyncGeneratorPrototype!;

    /// <summary>%AsyncFromSyncIteratorPrototype%</summary>
    public JsObject AsyncFromSyncIteratorPrototype => _asyncFromSyncIteratorPrototype!;

    /// <summary>%eval%</summary>
    public JsFunction Eval => _eval!;

    partial void InitializeGenerators()
    {}

    private JsBuiltinFunction CreateDynamicFunctionConstructor(string name, DynamicFunctionKind kind, JsObject prototype)
    {
        var constructor = Realm.CreateBuiltinFunction(name, 1, (function, _, arguments, newTarget) => DynamicFunctions.CreateDynamicFunction(function, newTarget, kind, arguments), isConstructor: true, prototype: Function);
        constructor.DefineBuiltIn("prototype", prototype, PropertyFlags.None);
        prototype.DefineBuiltIn("constructor", constructor, PropertyFlags.Configurable);
        return constructor;
    }

    private static JsGenerator GetGenerator(JsValue thisValue)
    {
        if (thisValue.Cell is JsGenerator generator)
            return generator;

        return ThrowHelper.ThrowTypeError<JsGenerator>("Generator method called on an incompatible receiver");
    }
}
