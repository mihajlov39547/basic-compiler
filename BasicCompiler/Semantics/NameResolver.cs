using BasicCompiler.Syntax.AST;

namespace BasicCompiler.Semantics;

public sealed class NameResolver
{
    private readonly Scope _globalScope = new();
    private readonly List<Symbol> _declaredSymbols = [];
    private Scope _currentScope;

    public NameResolver()
    {
        _currentScope = _globalScope;
    }

    public string GetSymbolDump()
    {
        var symbols = _declaredSymbols
            .DistinctBy(symbol => symbol.Name)
            .OrderBy(symbol => symbol.Name)
            .ToList();

        if (symbols.Count == 0)
        {
            return "No symbols declared.";
        }

        return string.Join(Environment.NewLine, symbols.Select(symbol => $"{symbol.Name}: {symbol.TypeName}"));
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
                var symbol = new VariableSymbol(declaration.Name, "unknown");
                _currentScope.Declare(symbol);
                _declaredSymbols.Add(symbol);
                if (declaration.Initializer is not null)
                {
                    ResolveExpression(declaration.Initializer);
                }
                break;

            case BlockStatement block:
                ResolveBlock(block);
                break;

            case IfStatement ifStatement:
                ResolveExpression(ifStatement.Condition);
                ResolveStatement(ifStatement.ThenStatement);
                if (ifStatement.ElseStatement is not null)
                {
                    ResolveStatement(ifStatement.ElseStatement);
                }
                break;

            case WhileStatement whileStatement:
                ResolveExpression(whileStatement.Condition);
                ResolveStatement(whileStatement.Body);
                break;

            case PrintStatement printStatement:
                ResolveExpression(printStatement.Expression);
                break;
        }
    }

    private void ResolveBlock(BlockStatement block)
    {
        var parentScope = _currentScope;
        _currentScope = new Scope(parentScope);

        try
        {
            foreach (var statement in block.Statements)
            {
                ResolveStatement(statement);
            }
        }
        finally
        {
            _currentScope = parentScope;
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

            case UnaryExpression unary:
                ResolveExpression(unary.Operand);
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
