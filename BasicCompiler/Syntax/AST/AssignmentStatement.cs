namespace BasicCompiler.Syntax.AST;

public sealed class AssignmentStatement : Statement
{
    public AssignmentStatement(string name, Expression expression)
    {
        Name = name;
        Expression = expression;
    }

    public string Name { get; }
    public Expression Expression { get; }

    public override string ToString() => $"{Name} := {Expression};";
}
