namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>The WeakRef and FinalizationRegistry built-ins are created on first use (a global property, or an intrinsic).</summary>
internal sealed partial class Intrinsics
{
    private JsFunction? _weakRef;
    private JsObject? _weakRefPrototype;
    private JsFunction? _finalizationRegistry;
    private JsObject? _finalizationRegistryPrototype;
    private bool _isWeakRefsInitializing;

    /// <summary>%WeakRef%</summary>
    public JsFunction WeakRef => EnsureWeakRefs()._weakRef!;

    /// <summary>%WeakRef.prototype%</summary>
    public JsObject WeakRefPrototype => EnsureWeakRefs()._weakRefPrototype!;

    /// <summary>%FinalizationRegistry%</summary>
    public JsFunction FinalizationRegistry => EnsureWeakRefs()._finalizationRegistry!;

    /// <summary>%FinalizationRegistry.prototype%</summary>
    public JsObject FinalizationRegistryPrototype => EnsureWeakRefs()._finalizationRegistryPrototype!;

    partial void InitializeWeakRefs()
    {}

    /// <summary>Creates the WeakRef and FinalizationRegistry built-ins, if they are not created yet.</summary>
    private Intrinsics EnsureWeakRefs()
    {
        if (_weakRef is null && !_isWeakRefsInitializing)
        {
            _isWeakRefsInitializing = true;
            _weakRefPrototype = new JsObject(ObjectPrototype);
            _finalizationRegistryPrototype = new JsObject(ObjectPrototype);
        }

        return this;
    }
}
