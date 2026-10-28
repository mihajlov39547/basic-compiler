namespace BasicCompiler.Diagnostics;

public sealed class DiagnosticBag
{
    private readonly List<Diagnostic> _diagnostics = new();

    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

    public void Add(Diagnostic diagnostic)
    {
        if (diagnostic is null)
        {
            throw new ArgumentNullException(nameof(diagnostic));
        }

        _diagnostics.Add(diagnostic);
    }

    public void AddRange(IEnumerable<Diagnostic> diagnostics)
    {
        if (diagnostics is null)
        {
            throw new ArgumentNullException(nameof(diagnostics));
        }

        _diagnostics.AddRange(diagnostics);
    }

    public void ReportError(string message, SourceSpan span)
    {
        Add(new Diagnostic(message, DiagnosticSeverity.Error, span));
    }

    public void ReportWarning(string message, SourceSpan span)
    {
        Add(new Diagnostic(message, DiagnosticSeverity.Warning, span));
    }

    public void ReportInfo(string message, SourceSpan span)
    {
        Add(new Diagnostic(message, DiagnosticSeverity.Info, span));
    }

    public void Clear() => _diagnostics.Clear();

    public override string ToString()
    {
        if (_diagnostics.Count == 0)
        {
            return "No diagnostics.";
        }

        return string.Join(Environment.NewLine, _diagnostics.Select(d => d.ToString()));
    }
}
