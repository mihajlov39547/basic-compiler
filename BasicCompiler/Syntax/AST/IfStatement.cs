namespace BasicCompiler.Syntax.AST;

public sealed class IfStatement : Statement
{
    public IfStatement(Expression condition, Statement thenStatement, Statement? elseStatement = null)
    {
        Condition = condition;
        ThenStatement = thenStatement;
        ElseStatement = elseStatement;
    }

    public Expression Condition { get; }
    public Statement ThenStatement { get; }
    public Statement? ElseStatement { get; }

    public override string ToString() => ElseStatement is null
        ? $"if ({Condition}) then {ThenStatement};"
        : $"if ({Condition}) then {ThenStatement} else {ElseStatement};";
}
