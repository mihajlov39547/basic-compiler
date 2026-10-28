namespace BasicCompiler.Syntax.AST;

public sealed class LiteralExpression : Expression
{
    public LiteralExpression(object value)
    {
        Value = value;
    }

    public object Value { get; }

    public override string ToString() => Value.ToString() ?? "";
}
