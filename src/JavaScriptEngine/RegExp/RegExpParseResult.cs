namespace Meziantou.Framework.JavaScriptEngine;

internal sealed record RegExpParseResult(RegExpNode Root, int GroupCount, string?[] GroupNames);
