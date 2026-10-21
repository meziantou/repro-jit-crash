namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>
/// The ArrayBuffer, DataView, typed array and Atomics built-ins are created on first use (a global property, or an
/// intrinsic used by another built-in or by the host): they are about 15% of the built-ins of a realm, and most scripts do
/// not use them. The creation is not observable. SharedArrayBuffer is created with the realm: it is small, and a host
/// reads it when it creates a realm (HTML deletes the global property, and keeps the intrinsic).
/// </summary>
internal sealed partial class Intrinsics
{
}
