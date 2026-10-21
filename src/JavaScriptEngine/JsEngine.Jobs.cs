using System.Collections.Concurrent;

namespace Meziantou.Framework.JavaScriptEngine;

// The job queue of the agent (https://tc39.es/ecma262/#sec-jobs) and the [[KeptAlive]] list of the WeakRefs.
public sealed partial class JsEngine
{

    /// <summary>Gets or sets the host-defined operations (job scheduling, rejection tracking, job callbacks, error reporting).</summary>
    internal JsHost HostHooks { get; set; }
}
