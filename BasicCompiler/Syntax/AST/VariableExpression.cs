using BasicCompiler.Semantics;

namespace BasicCompiler.Syntax.AST;

public sealed class VariableExpression : Expression
{
    public VariableExpression(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public VariableSymbol? BoundSymbol { get; set; }
    public bool IsBound => BoundSymbol is not null;

    public override TResult Accept<TResult>(IExpressionVisitor<TResult> visitor) => visitor.Visit(this);

    public override string ToString() => Name;
}
