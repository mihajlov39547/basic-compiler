namespace BasicCompiler.Diagnostics;

public sealed record Diagnostic(string Message, DiagnosticSeverity Severity, SourceSpan Span)
{
    public override string ToString() => $"[{Severity}] {Message} at {Span}";
}

public enum DiagnosticSeverity
{
    Info,
    Warning,
    Error
}
