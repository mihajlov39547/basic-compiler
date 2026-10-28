namespace BasicCompiler.Syntax.AST;

public abstract class Statement
{
    public abstract TResult Accept<TResult>(IStatementVisitor<TResult> visitor);

    public abstract override string ToString();
}
