namespace BasicCompiler.Syntax.AST;

public sealed class PrintStatement : Statement
{
    public PrintStatement(Expression expression)
    {
        Expression = expression;
    }

    public Expression Expression { get; }

    public override TResult Accept<TResult>(IStatementVisitor<TResult> visitor) => visitor.Visit(this);

    public override string ToString() => $"print {Expression};";
}
