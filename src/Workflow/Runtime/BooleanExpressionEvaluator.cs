namespace Workflow.Runtime;

internal static class BooleanExpressionEvaluator
{
    // Minimál, de elég a te YAML-edhez:
    // - var == true/false
    // - &&, ||
    // - zárójelek (alap)
    // Ismeretlen változó = false
    public static bool Eval(string expr, IReadOnlyDictionary<string, bool> vars)
    {
        if (string.IsNullOrWhiteSpace(expr))
            throw new ArgumentException("Expression is empty.", nameof(expr));

        expr = expr.Trim();
        expr = StripOuterParens(expr);

        // OR split top-level
        var orParts = SplitTopLevel(expr, "||");
        if (orParts.Count > 1)
            return orParts.Any(p => Eval(p, vars));

        // AND split top-level
        var andParts = SplitTopLevel(expr, "&&");
        if (andParts.Count > 1)
            return andParts.All(p => Eval(p, vars));

        // comparison: name == true/false
        var s = expr.Trim();

        // allow bare variable: "termination_needed"
        if (!s.Contains("==", StringComparison.Ordinal))
        {
            var key = s.Trim();
            return vars.TryGetValue(key, out var v) && v;
        }

        var idx = s.IndexOf("==", StringComparison.Ordinal);
        var left = s[..idx].Trim();
        var right = s[(idx + 2)..].Trim();

        var expected = right.Equals("true", StringComparison.OrdinalIgnoreCase)
            ? true
            : right.Equals("false", StringComparison.OrdinalIgnoreCase)
                ? false
                : throw new InvalidOperationException($"Unsupported literal '{right}' in expression '{expr}'.");

        var actual = vars.TryGetValue(left, out var val) ? val : false;
        return actual == expected;
    }

    private static string StripOuterParens(string s)
    {
        s = s.Trim();
        while (s.StartsWith("(") && s.EndsWith(")"))
        {
            var inner = s[1..^1].Trim();
            if (!IsBalanced(inner)) break;
            s = inner;
        }
        return s;
    }

    private static bool IsBalanced(string s)
    {
        int depth = 0;
        foreach (var ch in s)
        {
            if (ch == '(') depth++;
            if (ch == ')')
            {
                depth--;
                if (depth < 0) return false;
            }
        }
        return depth == 0;
    }

    private static List<string> SplitTopLevel(string s, string op)
    {
        var parts = new List<string>();
        int depth = 0;
        int last = 0;

        for (int i = 0; i <= s.Length - op.Length; i++)
        {
            var ch = s[i];
            if (ch == '(') depth++;
            else if (ch == ')') depth--;

            if (depth == 0 && s.AsSpan(i, op.Length).SequenceEqual(op))
            {
                parts.Add(s[last..i].Trim());
                i += op.Length - 1;
                last = i + 1;
            }
        }

        parts.Add(s[last..].Trim());
        return parts.Where(p => p.Length > 0).ToList();
    }
}

