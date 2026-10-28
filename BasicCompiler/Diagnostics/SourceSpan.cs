namespace BasicCompiler.Diagnostics;

public readonly record struct SourceSpan(int Start, int Length, int Line, int Column)
{
    public int End => Start + Length;

    public override string ToString() => $"line {Line}, column {Column} (start={Start}, length={Length})";
}
