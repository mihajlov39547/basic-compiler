namespace BasicCompiler.Syntax.AST;

public abstract class Expression
{
    public abstract TResult Accept<TResult>(IExpressionVisitor<TResult> visitor);

    public abstract override string ToString();
}
