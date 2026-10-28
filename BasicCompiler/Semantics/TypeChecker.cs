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
                scope.Declare(new VariableSymbol(declaration.Name, declaredType));
                ValidateExpression(declaration.Initializer, scope, diagnostics, $"variable '{declaration.Name}' initializer");
                break;

            case AssignmentStatement assignment:
                if (!scope.TryResolve(assignment.Name, out var symbol))
                {
                    diagnostics.Add(new Diagnostic($"Variable '{assignment.Name}' is not declared.", DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0)));
                    break;
                }

                var assignedType = InferType(assignment.Expression, scope);
                if (assignedType != "unknown" && symbol!.TypeName != "unknown" && assignedType != symbol.TypeName)
                {
                    diagnostics.Add(new Diagnostic($"Assignment type mismatch: variable '{assignment.Name}' is '{symbol.TypeName}' but expression is '{assignedType}'.", DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0)));
                }

                ValidateExpression(assignment.Expression, scope, diagnostics, $"assignment to '{assignment.Name}'");
                break;

            case IfStatement ifStatement:
                var conditionType = InferType(ifStatement.Condition, scope);
                if (conditionType != "bool")
                {
                    diagnostics.Add(new Diagnostic("If condition must evaluate to bool.", DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0)));
                }

                ValidateExpression(ifStatement.Condition, scope, diagnostics, "if condition");
                CheckStatement(ifStatement.ThenStatement, scope, diagnostics);
                if (ifStatement.ElseStatement is not null)
                {
                    CheckStatement(ifStatement.ElseStatement, scope, diagnostics);
                }
                break;

            case WhileStatement whileStatement:
                var loopConditionType = InferType(whileStatement.Condition, scope);
                if (loopConditionType != "bool")
                {
                    diagnostics.Add(new Diagnostic("While condition must evaluate to bool.", DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0)));
                }

                ValidateExpression(whileStatement.Condition, scope, diagnostics, "while condition");
                CheckStatement(whileStatement.Body, scope, diagnostics);
                break;

            case BlockStatement blockStatement:
                foreach (var child in blockStatement.Statements)
                {
                    CheckStatement(child, scope, diagnostics);
                }
                break;

            case PrintStatement printStatement:
                ValidateExpression(printStatement.Expression, scope, diagnostics, "print expression");
                break;
        }
    }

    private static void ValidateExpression(Expression? expression, Scope scope, ICollection<Diagnostic> diagnostics, string context)
    {
        if (expression is null)
        {
            return;
        }

        switch (expression)
        {
            case LiteralExpression:
                break;

            case VariableExpression variable:
                if (!scope.TryResolve(variable.Name, out _))
                {
                    diagnostics.Add(new Diagnostic($"Variable '{variable.Name}' is not declared.", DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0)));
                }
                break;

            case UnaryExpression unary:
                var operandType = InferType(unary.Operand, scope);
                switch (unary.Operator)
                {
                    case "!":
                        if (operandType != "bool")
                        {
                            diagnostics.Add(new Diagnostic($"Operator '{unary.Operator}' requires a boolean operand in {context}.", DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0)));
                        }
                        break;

                    case "-":
                        if (operandType != "number")
                        {
                            diagnostics.Add(new Diagnostic($"Operator '{unary.Operator}' requires a numeric operand in {context}.", DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0)));
                        }
                        break;
                }
                break;

            case BinaryExpression binary:
                var leftType = InferType(binary.Left, scope);
                var rightType = InferType(binary.Right, scope);

                switch (binary.Operator)
                {
                    case "+":
                        if ((leftType == "number" && rightType == "number") || (leftType == "string" && rightType == "string"))
                        {
                            break;
                        }

                        diagnostics.Add(new Diagnostic($"Operator '{binary.Operator}' requires numeric operands or string concatenation in {context}.", DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0)));
                        break;

                    case "-" or "*" or "/":
                        if (leftType != "number" || rightType != "number")
                        {
                            diagnostics.Add(new Diagnostic($"Operator '{binary.Operator}' requires numeric operands in {context}.", DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0)));
                        }
                        break;

                    case "&&" or "||":
                        if (leftType != "bool" || rightType != "bool")
                        {
                            diagnostics.Add(new Diagnostic($"Operator '{binary.Operator}' requires boolean operands in {context}.", DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0)));
                        }
                        break;

                    case "==" or "=" or "!=":
                        if (leftType != "unknown" && rightType != "unknown" && leftType != rightType)
                        {
                            diagnostics.Add(new Diagnostic($"Operator '{binary.Operator}' requires matching operand types in {context}.", DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0)));
                        }
                        break;

                    case "<" or "<=" or ">" or ">=":
                        if (leftType != "number" || rightType != "number")
                        {
                            diagnostics.Add(new Diagnostic($"Operator '{binary.Operator}' requires numeric operands in {context}.", DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0)));
                        }
                        break;
                }

                ValidateExpression(binary.Left, scope, diagnostics, context);
                ValidateExpression(binary.Right, scope, diagnostics, context);
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

            case UnaryExpression unary:
                var operandType = InferType(unary.Operand, scope);
                return unary.Operator switch
                {
                    "!" when operandType == "bool" => "bool",
                    "-" when operandType == "number" => "number",
                    _ => "unknown"
                };

            case BinaryExpression binary:
                var leftType = InferType(binary.Left, scope);
                var rightType = InferType(binary.Right, scope);

                if (binary.Operator == "+")
                {
                    if (leftType == "number" && rightType == "number")
                    {
                        return "number";
                    }

                    if (leftType == "string" && rightType == "string")
                    {
                        return "string";
                    }

                    return "unknown";
                }

                if (binary.Operator is "-" or "*" or "/" or "<" or "<=" or ">" or ">=")
                {
                    return leftType == "number" && rightType == "number" ?
                        binary.Operator is "<" or "<=" or ">" or ">=" ? "bool" : "number" :
                        "unknown";
                }

                if (binary.Operator is "==" or "=" or "!=")
                {
                    return leftType == rightType && leftType != "unknown" ? "bool" : "unknown";
                }

                if (binary.Operator is "&&" or "||")
                {
                    return leftType == "bool" && rightType == "bool" ? "bool" : "unknown";
                }

                return "unknown";

            default:
                return "unknown";
        }
    }
}
