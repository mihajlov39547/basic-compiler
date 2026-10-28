using BasicCompiler.Syntax.AST;

namespace BasicCompiler.Runtime;

public sealed class Interpreter
{
    private readonly Environment _globalEnvironment = new();

    public object? Execute(IEnumerable<Statement> statements)
    {
        object? lastValue = null;

        foreach (var statement in statements)
        {
            lastValue = ExecuteStatement(statement);
        }

        return lastValue;
    }

    private object? ExecuteStatement(Statement statement)
    {
        switch (statement)
        {
            case VariableDeclaration declaration:
                {
                    var value = declaration.Initializer is null ? null : EvaluateExpression(declaration.Initializer);
                    _globalEnvironment.Define(declaration.Name, value);
                    return value;
                }

            case IfStatement ifStatement:
                {
                    var condition = EvaluateExpression(ifStatement.Condition);
                    if (condition is bool boolValue && boolValue)
                    {
                        return ExecuteStatement(ifStatement.ThenStatement);
                    }

                    if (ifStatement.ElseStatement is not null)
                    {
                        return ExecuteStatement(ifStatement.ElseStatement);
                    }

                    return null;
                }

            case PrintStatement printStatement:
                {
                    var value = EvaluateExpression(printStatement.Expression);
                    Console.WriteLine(value);
                    return value;
                }

            default:
                throw new InvalidOperationException($"Statement type {statement.GetType().Name} is not supported.");
        }
    }

    private object? EvaluateExpression(Expression expression)
    {
        switch (expression)
        {
            case LiteralExpression literal:
                return literal.Value;

            case VariableExpression variable:
                return _globalEnvironment.Get(variable.Name);

            case BinaryExpression binary:
                var left = EvaluateExpression(binary.Left);
                var right = EvaluateExpression(binary.Right);

                return binary.Operator switch
                {
                    "+" => Add(left, right),
                    "-" => Subtract(left, right),
                    "*" => Multiply(left, right),
                    "/" => Divide(left, right),
                    "==" => AreEqual(left, right),
                    "!=" => !AreEqual(left, right),
                    "<" => Compare(left, right, (a, b) => a < b),
                    ">" => Compare(left, right, (a, b) => a > b),
                    "<=" => Compare(left, right, (a, b) => a <= b),
                    ">=" => Compare(left, right, (a, b) => a >= b),
                    _ => throw new InvalidOperationException($"Unsupported operator '{binary.Operator}'.")
                };

            default:
                throw new InvalidOperationException($"Expression type {expression.GetType().Name} is not supported.");
        }
    }

    private static object? Add(object? left, object? right)
    {
        return left switch
        {
            double d when right is double r => d + r,
            int i when right is int j => i + j,
            _ => throw new InvalidOperationException("Addition requires numeric operands.")
        };
    }

    private static object? Subtract(object? left, object? right)
    {
        if (left is double d && right is double r) return d - r;
        if (left is int i && right is int j) return i - j;
        throw new InvalidOperationException("Subtraction requires numeric operands.");
    }

    private static object? Multiply(object? left, object? right)
    {
        if (left is double d && right is double r) return d * r;
        if (left is int i && right is int j) return i * j;
        throw new InvalidOperationException("Multiplication requires numeric operands.");
    }

    private static object? Divide(object? left, object? right)
    {
        if (left is double d && right is double r) return d / r;
        if (left is int i && right is int j) return i / (double)j;
        throw new InvalidOperationException("Division requires numeric operands.");
    }

    private static bool AreEqual(object? left, object? right) => left?.Equals(right) ?? right is null;

    private static bool Compare(object? left, object? right, Func<double, double, bool> operation)
    {
        if (left is double d && right is double r)
        {
            return operation(d, r);
        }

        if (left is int i && right is int j)
        {
            return operation(i, j);
        }

        throw new InvalidOperationException("Comparison requires numeric operands.");
    }
}
