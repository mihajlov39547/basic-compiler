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
            case BlockStatement block:
                object? blockResult = null;
                foreach (var item in block.Statements)
                {
                    blockResult = ExecuteStatement(item);
                }
                return blockResult;

            case VariableDeclaration declaration:
                {
                    var value = declaration.Initializer is null ? null : EvaluateExpression(declaration.Initializer);
                    _globalEnvironment.Define(declaration.Name, value);
                    return value;
                }

            case AssignmentStatement assignment:
                {
                    var value = EvaluateExpression(assignment.Expression);
                    _globalEnvironment.Set(assignment.Name, value);
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

            case WhileStatement whileStatement:
                {
                    while (EvaluateExpression(whileStatement.Condition) is bool loopCondition && loopCondition)
                    {
                        ExecuteStatement(whileStatement.Body);
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

            case UnaryExpression unary:
                var operand = EvaluateExpression(unary.Operand);
                return unary.Operator switch
                {
                    "!" => !(operand is bool value && value),
                    "-" => Negate(operand),
                    _ => throw new InvalidOperationException($"Unsupported unary operator '{unary.Operator}'.")
                };

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
                    "=" => AreEqual(left, right),
                    "!=" => !AreEqual(left, right),
                    "&&" => AreTrue(left) && AreTrue(right),
                    "||" => AreTrue(left) || AreTrue(right),
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

    private static object? Negate(object? value)
    {
        if (value is double d) return -d;
        if (value is int i) return -i;
        throw new InvalidOperationException("Unary minus requires a numeric operand.");
    }

    private static bool AreEqual(object? left, object? right) => left?.Equals(right) ?? right is null;

    private static bool AreTrue(object? value)
    {
        if (value is bool b)
        {
            return b;
        }

        throw new InvalidOperationException("Logical operators require boolean operands.");
    }

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
