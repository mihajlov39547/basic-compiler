namespace BasicCompiler.Syntax.AST;

public sealed class BinaryExpression : Expression
{
    public BinaryExpression(Expression left, string op, Expression right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }

    public Expression Left { get; }
    public string Operator { get; }
    public Expression Right { get; }

    public override TResult Accept<TResult>(IExpressionVisitor<TResult> visitor) => visitor.Visit(this);

    public override string ToString() => $"({Left} {Operator} {Right})";
}
