namespace BasicCompiler.Syntax.AST;

public sealed class BlockStatement : Statement
{
    public BlockStatement(IReadOnlyList<Statement> statements)
    {
        Statements = statements;
    }

    public IReadOnlyList<Statement> Statements { get; }

    public override string ToString() => string.Join(" ", Statements.Select(s => s.ToString()));
}
