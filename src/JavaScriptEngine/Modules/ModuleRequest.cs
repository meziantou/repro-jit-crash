using System.Diagnostics.CodeAnalysis;

namespace Meziantou.Framework.JavaScriptEngine.Modules;

/// <summary>The [[Phase]] of a module request or of a namespace object (https://tc39.es/proposal-defer-import-eval/).</summary>
internal enum ModulePhase
{
    /// <summary>The module is evaluated before its importer.</summary>
    Evaluation,

    /// <summary>The module is evaluated on the first access to an export of its deferred namespace object.</summary>
    Defer,

    /// <summary>The module is loaded, not linked nor evaluated: the import is the source of the module (https://tc39.es/proposal-source-phase-imports/).</summary>
    Source,
}
