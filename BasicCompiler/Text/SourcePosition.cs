namespace BasicCompiler.Text;

public readonly record struct SourcePosition(int Index, int Line, int Column)
{
    public override string ToString() => $"line {Line}, column {Column} (index={Index})";
}
