using System.Diagnostics.CodeAnalysis;

namespace Meziantou.Framework.JavaScriptEngine;

/// <summary>Throws the native errors, created in the current realm of the engine of the thread.</summary>
internal static class ThrowHelper
{

    /// <summary>Creates the exception of the SyntaxError of a source text that cannot be parsed, located at the error.</summary>
    public static JsException CreateSyntaxErrorException(JsSyntaxErrorException exception)
    {
        var realm = JsEngine.Current?.CurrentRealm;
        if (realm is null)
            throw new InvalidOperationException($"SyntaxError: {exception.Message} (no JavaScript engine is running on this thread)");

        var location = exception.Line > 0 ? new JsSourceLocation(exception.Url, exception.Line, exception.Column) : (JsSourceLocation?)null;
        return new JsException(new JsValue(realm.CreateError(ErrorType.SyntaxError, exception.Message)), location);
    }
}
