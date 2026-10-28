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
            var runtimeValue = ExecuteStatement(statement);
            lastValue = runtimeValue?.ToClrObject();
        }

        return lastValue;
    }

    private RuntimeValue ExecuteStatement(Statement statement)
    {
        switch (statement)
        {
            case BlockStatement block:
                RuntimeValue? blockResult = null;
                foreach (var item in block.Statements)
                {
                    blockResult = ExecuteStatement(item);
                }
                return blockResult ?? RuntimeValue.Boolean(true);

            case VariableDeclaration declaration:
                {
                    var value = declaration.Initializer is null ? null : EvaluateExpression(declaration.Initializer);
                    _globalEnvironment.Define(declaration.Name, value ?? RuntimeValue.Boolean(true));
                    return value ?? RuntimeValue.Boolean(true);
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
                    if (condition is BooleanValue { Value: true })
                    {
                        return ExecuteStatement(ifStatement.ThenStatement);
                    }

                    if (ifStatement.ElseStatement is not null)
                    {
                        return ExecuteStatement(ifStatement.ElseStatement);
                    }

                    return RuntimeValue.Boolean(true);
                }

            case WhileStatement whileStatement:
                {
                    while (EvaluateExpression(whileStatement.Condition) is BooleanValue { Value: true })
                    {
                        ExecuteStatement(whileStatement.Body);
                    }

                    return RuntimeValue.Boolean(true);
                }

            case PrintStatement printStatement:
                {
                    var value = EvaluateExpression(printStatement.Expression);
                    Console.WriteLine(value.ToClrObject());
                    return value;
                }

            default:
                throw new InvalidOperationException($"Statement type {statement.GetType().Name} is not supported.");
        }
    }

    private RuntimeValue EvaluateExpression(Expression expression)
    {
        switch (expression)
        {
            case LiteralExpression literal:
                return RuntimeValue.FromObject(literal.Value);

            case VariableExpression variable:
                return _globalEnvironment.Get(variable.Name);

            case UnaryExpression unary:
                var operand = EvaluateExpression(unary.Operand);
                return unary.Operator switch
                {
                    "!" => RuntimeValue.Boolean(!(operand is BooleanValue value && value.Value)),
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
                    "==" => RuntimeValue.Boolean(AreEqual(left, right)),
                    "=" => RuntimeValue.Boolean(AreEqual(left, right)),
                    "!=" => RuntimeValue.Boolean(!AreEqual(left, right)),
                    "&&" => RuntimeValue.Boolean(AreTrue(left) && AreTrue(right)),
                    "||" => RuntimeValue.Boolean(AreTrue(left) || AreTrue(right)),
                    "<" => RuntimeValue.Boolean(Compare(left, right, (a, b) => a < b)),
                    ">" => RuntimeValue.Boolean(Compare(left, right, (a, b) => a > b)),
                    "<=" => RuntimeValue.Boolean(Compare(left, right, (a, b) => a <= b)),
                    ">=" => RuntimeValue.Boolean(Compare(left, right, (a, b) => a >= b)),
                    _ => throw new InvalidOperationException($"Unsupported operator '{binary.Operator}'.")
                };

            default:
                throw new InvalidOperationException($"Expression type {expression.GetType().Name} is not supported.");
        }
    }

    private static RuntimeValue Add(RuntimeValue left, RuntimeValue right)
    {
        if (left is NumberValue leftNumber && right is NumberValue rightNumber)
        {
            return RuntimeValue.Number(leftNumber.Value + rightNumber.Value);
        }

        throw new InvalidOperationException("Addition requires numeric operands.");
    }

    private static RuntimeValue Subtract(RuntimeValue left, RuntimeValue right)
    {
        if (left is NumberValue leftNumber && right is NumberValue rightNumber)
        {
            return RuntimeValue.Number(leftNumber.Value - rightNumber.Value);
        }

        throw new InvalidOperationException("Subtraction requires numeric operands.");
    }

    private static RuntimeValue Multiply(RuntimeValue left, RuntimeValue right)
    {
        if (left is NumberValue leftNumber && right is NumberValue rightNumber)
        {
            return RuntimeValue.Number(leftNumber.Value * rightNumber.Value);
        }

        throw new InvalidOperationException("Multiplication requires numeric operands.");
    }

    private static RuntimeValue Divide(RuntimeValue left, RuntimeValue right)
    {
        if (left is NumberValue leftNumber && right is NumberValue rightNumber)
        {
            return RuntimeValue.Number(leftNumber.Value / rightNumber.Value);
        }

        throw new InvalidOperationException("Division requires numeric operands.");
    }

    private static RuntimeValue Negate(RuntimeValue value)
    {
        if (value is NumberValue numberValue)
        {
            return RuntimeValue.Number(-numberValue.Value);
        }

        throw new InvalidOperationException("Unary minus requires a numeric operand.");
    }

    private static bool AreEqual(RuntimeValue left, RuntimeValue right)
    {
        if (left is NumberValue leftNumber && right is NumberValue rightNumber)
        {
            return leftNumber.Value == rightNumber.Value;
        }

        if (left is StringValue leftString && right is StringValue rightString)
        {
            return leftString.Value == rightString.Value;
        }

        if (left is BooleanValue leftBool && right is BooleanValue rightBool)
        {
            return leftBool.Value == rightBool.Value;
        }

        return false;
    }

    private static bool AreTrue(RuntimeValue value)
    {
        if (value is BooleanValue booleanValue)
        {
            return booleanValue.Value;
        }

        throw new InvalidOperationException("Logical operators require boolean operands.");
    }

    private static bool Compare(RuntimeValue left, RuntimeValue right, Func<double, double, bool> operation)
    {
        if (left is NumberValue leftNumber && right is NumberValue rightNumber)
        {
            return operation(leftNumber.Value, rightNumber.Value);
        }

        throw new InvalidOperationException("Comparison requires numeric operands.");
    }
}
