namespace BasicCompiler.Syntax.AST;

public sealed class VariableDeclaration : Statement
{
    public VariableDeclaration(string name, Expression? initializer)
    {
        Name = name;
        Initializer = initializer;
    }

    public string Name { get; }
    public Expression? Initializer { get; }

    public override TResult Accept<TResult>(IStatementVisitor<TResult> visitor) => visitor.Visit(this);

    public override string ToString() => Initializer is null
        ? $"var {Name};"
        : $"var {Name} := {Initializer};";
}
