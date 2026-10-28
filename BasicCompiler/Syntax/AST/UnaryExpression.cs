namespace BasicCompiler.Syntax.AST;

public sealed class UnaryExpression : Expression
{
    public UnaryExpression(string op, Expression operand)
    {
        Operator = op;
        Operand = operand;
    }

    public string Operator { get; }
    public Expression Operand { get; }

    public override TResult Accept<TResult>(IExpressionVisitor<TResult> visitor) => visitor.Visit(this);

    public override string ToString() => $"{Operator}{Operand}";
}
