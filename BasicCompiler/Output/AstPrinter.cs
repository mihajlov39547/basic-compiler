using System.Text;
using BasicCompiler.Syntax.AST;

namespace BasicCompiler.Output;

public static class AstPrinter
{
    public static string Print(IReadOnlyList<Statement> statements)
    {
        var lines = new List<string> { "Program" };

        for (var i = 0; i < statements.Count; i++)
        {
            var isLast = i == statements.Count - 1;
            AppendStatement(lines, statements[i], string.Empty, isLast);
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static void AppendStatement(List<string> lines, Statement statement, string indent, bool isLast)
    {
        var connector = isLast ? "└── " : "├── ";
        lines.Add($"{indent}{connector}{GetNodeName(statement)}");

        var childIndent = indent + (isLast ? "    " : "│   ");

        switch (statement)
        {
            case VariableDeclaration variableDeclaration:
                lines.Add($"{childIndent}├── Name: {variableDeclaration.Name}");
                if (variableDeclaration.Initializer is null)
                {
                    break;
                }

                lines.Add($"{childIndent}└── Initializer");
                AppendExpression(lines, variableDeclaration.Initializer, childIndent + "    ", true);
                break;

            case AssignmentStatement assignmentStatement:
                lines.Add($"{childIndent}├── Name: {assignmentStatement.Name}");
                lines.Add($"{childIndent}└── Expression");
                AppendExpression(lines, assignmentStatement.Expression, childIndent + "    ", true);
                break;

            case IfStatement ifStatement:
                lines.Add($"{childIndent}├── Condition");
                AppendExpression(lines, ifStatement.Condition, childIndent + "│   ", true);

                lines.Add($"{childIndent}├── Then");
                AppendStatement(lines, ifStatement.ThenStatement, childIndent + "│   ", true);

                if (ifStatement.ElseStatement is not null)
                {
                    lines.Add($"{childIndent}└── Else");
                    AppendStatement(lines, ifStatement.ElseStatement, childIndent + "    ", true);
                }
                break;

            case WhileStatement whileStatement:
                lines.Add($"{childIndent}├── Condition");
                AppendExpression(lines, whileStatement.Condition, childIndent + "│   ", true);
                lines.Add($"{childIndent}└── Body");
                AppendStatement(lines, whileStatement.Body, childIndent + "    ", true);
                break;

            case PrintStatement printStatement:
                lines.Add($"{childIndent}└── Expression");
                AppendExpression(lines, printStatement.Expression, childIndent + "    ", true);
                break;

            case BlockStatement blockStatement:
                for (var i = 0; i < blockStatement.Statements.Count; i++)
                {
                    AppendStatement(lines, blockStatement.Statements[i], childIndent, i == blockStatement.Statements.Count - 1);
                }
                break;
        }
    }

    private static void AppendExpression(List<string> lines, Expression expression, string indent, bool isLast)
    {
        var connector = isLast ? "└── " : "├── ";
        lines.Add($"{indent}{connector}{GetExpressionLabel(expression)}");

        var childIndent = indent + (isLast ? "    " : "│   ");

        switch (expression)
        {
            case BinaryExpression binaryExpression:
                AppendExpression(lines, binaryExpression.Left, childIndent, false);
                AppendExpression(lines, binaryExpression.Right, childIndent, true);
                break;

            case UnaryExpression unaryExpression:
                AppendExpression(lines, unaryExpression.Operand, childIndent, true);
                break;

            case VariableExpression variableExpression:
                break;

            case LiteralExpression literalExpression:
                break;
        }
    }

    private static string GetNodeName(Statement statement) => statement switch
    {
        VariableDeclaration => "VariableDeclaration",
        AssignmentStatement => "AssignmentStatement",
        IfStatement => "IfStatement",
        WhileStatement => "WhileStatement",
        PrintStatement => "PrintStatement",
        BlockStatement => "BlockStatement",
        _ => statement.GetType().Name
    };

    private static string GetExpressionLabel(Expression expression) => expression switch
    {
        BinaryExpression binaryExpression => $"BinaryExpression [{MapOperator(binaryExpression.Operator)}]",
        UnaryExpression unaryExpression => $"UnaryExpression [{MapUnaryOperator(unaryExpression.Operator)}]",
        VariableExpression variableExpression => $"VariableExpression [{variableExpression.Name}]",
        LiteralExpression literalExpression => GetLiteralLabel(literalExpression.Value),
        _ => expression.GetType().Name
    };

    private static string GetLiteralLabel(object value) => value switch
    {
        string text =>
            $"StringLiteral [\"{text.Trim('"')}\"]",
        bool boolean => $"BooleanLiteral [{boolean.ToString().ToLowerInvariant()}]",
        double or float or decimal => $"NumberLiteral [{value}]",
        int => $"NumberLiteral [{value}]",
        long => $"NumberLiteral [{value}]",
        _ => $"Literal [{value}]"
    };

    private static string MapOperator(string op) => op switch
    {
        "+" => "Add",
        "-" => "Subtract",
        "*" => "Multiply",
        "/" => "Divide",
        "==" => "Equal",
        "=" => "Equal",
        "!=" => "NotEqual",
        "<" => "LessThan",
        "<=" => "LessThanOrEqual",
        ">" => "GreaterThan",
        ">=" => "GreaterThanOrEqual",
        "&&" => "LogicalAnd",
        "||" => "LogicalOr",
        _ => op
    };

    private static string MapUnaryOperator(string op) => op switch
    {
        "!" => "LogicalNot",
        "-" => "Negate",
        _ => op
    };
}
