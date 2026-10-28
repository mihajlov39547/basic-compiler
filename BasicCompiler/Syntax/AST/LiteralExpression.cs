namespace BasicCompiler.Syntax.AST;

public sealed class LiteralExpression : Expression
{
    public LiteralExpression(object value)
    {
        Value = value;
    }

    public object Value { get; }

    public override TResult Accept<TResult>(IExpressionVisitor<TResult> visitor) => visitor.Visit(this);

    public override string ToString() => Value.ToString() ?? "";
}
