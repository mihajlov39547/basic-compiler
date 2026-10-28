namespace BasicCompiler.Syntax.AST;

public sealed class WhileStatement : Statement
{
    public WhileStatement(Expression condition, Statement body)
    {
        Condition = condition;
        Body = body;
    }

    public Expression Condition { get; }
    public Statement Body { get; }

    public override string ToString() => $"while ({Condition}) do {Body};";
}
