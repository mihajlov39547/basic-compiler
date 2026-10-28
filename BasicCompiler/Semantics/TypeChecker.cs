using BasicCompiler.Diagnostics;
using BasicCompiler.Syntax.AST;

namespace BasicCompiler.Semantics;

public sealed class TypeChecker
{
    public IReadOnlyList<Diagnostic> Check(IEnumerable<Statement> statements)
    {
        var diagnostics = new List<Diagnostic>();
        var scope = new Scope();

        foreach (var statement in statements)
        {
            CheckStatement(statement, scope, diagnostics);
        }

        return diagnostics;
    }

    private static void CheckStatement(Statement statement, Scope scope, ICollection<Diagnostic> diagnostics)
    {
        switch (statement)
        {
            case VariableDeclaration declaration:
                var declaredType = declaration.Initializer is null ? "unknown" : InferType(declaration.Initializer, scope);
                scope.Declare(new Symbol(declaration.Name, declaredType));
                break;

            case IfStatement ifStatement:
                var conditionType = InferType(ifStatement.Condition, scope);
                if (conditionType != "bool")
                {
                    diagnostics.Add(new Diagnostic("If condition must evaluate to bool.", DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0)));
                }

                CheckStatement(ifStatement.ThenStatement, scope, diagnostics);
                if (ifStatement.ElseStatement is not null)
                {
                    CheckStatement(ifStatement.ElseStatement, scope, diagnostics);
                }
                break;

            case PrintStatement printStatement:
                _ = InferType(printStatement.Expression, scope);
                break;
        }
    }

    private static string InferType(Expression expression, Scope scope)
    {
        switch (expression)
        {
            case LiteralExpression literal:
                return literal.Value switch
                {
                    double => "number",
                    bool => "bool",
                    string => "string",
                    _ => "unknown"
                };

            case VariableExpression variable:
                return scope.TryResolve(variable.Name, out var symbol)
                    ? symbol!.TypeName
                    : "unknown";

            case BinaryExpression binary:
                var leftType = InferType(binary.Left, scope);
                var rightType = InferType(binary.Right, scope);

                if (leftType == rightType)
                {
                    return leftType;
                }

                return "unknown";

            default:
                return "unknown";
        }
    }
}
