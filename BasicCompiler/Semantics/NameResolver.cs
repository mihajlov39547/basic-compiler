using BasicCompiler.Syntax.AST;

namespace BasicCompiler.Semantics;

public sealed class NameResolver
{
    private readonly Scope _globalScope = new();
    private Scope _currentScope;

    public NameResolver()
    {
        _currentScope = _globalScope;
    }

    public void Resolve(IEnumerable<Statement> statements)
    {
        foreach (var statement in statements)
        {
            ResolveStatement(statement);
        }
    }

    private void ResolveStatement(Statement statement)
    {
        switch (statement)
        {
            case VariableDeclaration declaration:
                _currentScope.Declare(new Symbol(declaration.Name, "unknown"));
                if (declaration.Initializer is not null)
                {
                    ResolveExpression(declaration.Initializer);
                }
                break;

            case IfStatement ifStatement:
                ResolveExpression(ifStatement.Condition);
                ResolveStatement(ifStatement.ThenStatement);
                if (ifStatement.ElseStatement is not null)
                {
                    ResolveStatement(ifStatement.ElseStatement);
                }
                break;

            case PrintStatement printStatement:
                ResolveExpression(printStatement.Expression);
                break;
        }
    }

    private void ResolveExpression(Expression expression)
    {
        switch (expression)
        {
            case BinaryExpression binary:
                ResolveExpression(binary.Left);
                ResolveExpression(binary.Right);
                break;

            case VariableExpression variable:
                if (!_currentScope.TryResolve(variable.Name, out _))
                {
                    throw new InvalidOperationException($"Variable '{variable.Name}' is not declared.");
                }
                break;

            case LiteralExpression:
                break;
        }
    }
}
