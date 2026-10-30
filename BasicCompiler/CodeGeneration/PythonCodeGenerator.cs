using System.Text;
using BasicCompiler.Syntax.AST;

namespace BasicCompiler.CodeGeneration;

public sealed class PythonCodeGenerator
{
    public string Generate(IReadOnlyList<Statement> statements)
    {
        var builder = new StringBuilder();

        foreach (var statement in statements)
        {
            EmitStatement(statement, builder, 0);
        }

        var result = builder.ToString().TrimEnd();
        return string.IsNullOrWhiteSpace(result) ? string.Empty : result + Environment.NewLine;
    }

    private static void EmitStatement(Statement statement, StringBuilder builder, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 4);

        switch (statement)
        {
            case VariableDeclaration declaration:
                var initializer = declaration.Initializer is null ? "None" : EmitExpression(declaration.Initializer);
                builder.AppendLine($"{indent}{declaration.Name} = {initializer}");
                break;

            case AssignmentStatement assignment:
                builder.AppendLine($"{indent}{assignment.Name} = {EmitExpression(assignment.Expression)}");
                break;

            case PrintStatement printStatement:
                builder.AppendLine($"{indent}print({EmitExpression(printStatement.Expression)})");
                break;

            case BlockStatement block:
                foreach (var nested in block.Statements)
                {
                    EmitStatement(nested, builder, indentLevel);
                }
                break;

            case IfStatement ifStatement:
                builder.AppendLine($"{indent}if {EmitExpression(ifStatement.Condition)}:");
                EmitStatement(ifStatement.ThenStatement, builder, indentLevel + 1);

                if (ifStatement.ElseStatement is not null)
                {
                    builder.AppendLine($"{indent}else:");
                    EmitStatement(ifStatement.ElseStatement, builder, indentLevel + 1);
                }
                break;

            case WhileStatement whileStatement:
                builder.AppendLine($"{indent}while {EmitExpression(whileStatement.Condition)}:");
                EmitStatement(whileStatement.Body, builder, indentLevel + 1);
                break;

            default:
                throw new InvalidOperationException($"Unsupported statement type '{statement.GetType().Name}' for Python generation.");
        }
    }

    private static string EmitExpression(Expression expression)
    {
        switch (expression)
        {
            case LiteralExpression literal:
                return FormatLiteral(literal.Value);

            case VariableExpression variable:
                return variable.Name;

            case UnaryExpression unary:
                var operand = EmitExpression(unary.Operand);
                return unary.Operator switch
                {
                    "!" => $"not ({operand})",
                    "-" => $"-({operand})",
                    "+" => $"+({operand})",
                    _ => $"{unary.Operator}({operand})"
                };

            case BinaryExpression binary:
                var left = EmitExpression(binary.Left);
                var right = EmitExpression(binary.Right);
                return $"{left} {MapOperator(binary.Operator)} {right}";

            default:
                throw new InvalidOperationException($"Unsupported expression type '{expression.GetType().Name}' for Python generation.");
        }
    }

    private static string FormatLiteral(object value)
    {
        return value switch
        {
            bool boolean => boolean ? "True" : "False",
            string text => $"\"{EscapeString(text)}\"",
            char ch => $"\"{EscapeString(ch.ToString())}\"",
            null => "None",
            double d when Math.Abs(d % 1) < double.Epsilon => d.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture),
            float f when Math.Abs(f % 1) < float.Epsilon => f.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture),
            decimal dec when dec % 1 == 0 => dec.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture),
            _ => Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? "None"
        };
    }

    private static string EscapeString(string value)
    {
        var escaped = value.Replace("\\", "\\\\");
        escaped = escaped.Replace("\"", "\\\"");
        escaped = escaped.Replace("\n", "\\n");
        escaped = escaped.Replace("\r", "\\r");
        escaped = escaped.Replace("\t", "\\t");
        return escaped;
    }

    private static string MapOperator(string op) => op switch
    {
        "&&" => "and",
        "||" => "or",
        "==" => "==",
        "!=" => "!=",
        "<" => "<",
        ">" => ">",
        "<=" => "<=",
        ">=" => ">=",
        "+" => "+",
        "-" => "-",
        "*" => "*",
        "/" => "/",
        _ => op
    };
}
