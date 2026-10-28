using System.Globalization;
using BasicCompiler.Syntax.AST;

namespace BasicCompiler.Output;

public sealed class AstDumpOptions
{
    public bool IncludeSourceLocations { get; init; }
}

public static class AstPrinter
{
    public static string Print(IReadOnlyList<Statement> statements, AstDumpOptions? options = null)
    {
        options ??= new AstDumpOptions();
        var lines = new List<string> { "Program" };

        for (var i = 0; i < statements.Count; i++)
        {
            AppendStatement(lines, statements[i], string.Empty, i == statements.Count - 1, options);
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static void AppendStatement(List<string> lines, Statement statement, string indent, bool isLast, AstDumpOptions options)
    {
        var connector = isLast ? "└── " : "├── ";
        lines.Add($"{indent}{connector}{GetNodeName(statement)}");

        var childIndent = indent + (isLast ? "    " : "│   ");

        switch (statement)
        {
            case VariableDeclaration variableDeclaration:
                lines.Add($"{childIndent}├── Name: {variableDeclaration.Name}");
                if (variableDeclaration.Initializer is not null)
                {
                    lines.Add($"{childIndent}└── Initializer");
                    AppendExpression(lines, variableDeclaration.Initializer, childIndent + "    ", true, options);
                }
                break;

            case AssignmentStatement assignmentStatement:
                lines.Add($"{childIndent}├── Target");
                AppendExpression(lines, new VariableExpression(assignmentStatement.Name), childIndent + "│   ", true, options);
                lines.Add($"{childIndent}└── Value");
                AppendExpression(lines, assignmentStatement.Expression, childIndent + "    ", true, options);
                break;

            case IfStatement ifStatement:
                lines.Add($"{childIndent}├── Condition");
                AppendExpression(lines, ifStatement.Condition, childIndent + "│   ", true, options);

                lines.Add($"{childIndent}├── Then");
                AppendStatement(lines, ifStatement.ThenStatement, childIndent + "│   ", true, options);

                if (ifStatement.ElseStatement is not null)
                {
                    lines.Add($"{childIndent}└── Else");
                    AppendStatement(lines, ifStatement.ElseStatement, childIndent + "    ", true, options);
                }
                break;

            case WhileStatement whileStatement:
                lines.Add($"{childIndent}├── Condition");
                AppendExpression(lines, whileStatement.Condition, childIndent + "│   ", true, options);
                lines.Add($"{childIndent}└── Body");
                AppendStatement(lines, whileStatement.Body, childIndent + "    ", true, options);
                break;

            case PrintStatement printStatement:
                AppendExpression(lines, printStatement.Expression, childIndent, true, options);
                break;

            case BlockStatement blockStatement:
                for (var i = 0; i < blockStatement.Statements.Count; i++)
                {
                    AppendStatement(lines, blockStatement.Statements[i], childIndent, i == blockStatement.Statements.Count - 1, options);
                }
                break;
        }
    }

    private static void AppendExpression(List<string> lines, Expression expression, string indent, bool isLast, AstDumpOptions options)
    {
        var connector = isLast ? "└── " : "├── ";
        var label = GetExpressionLabel(expression, options);
        lines.Add($"{indent}{connector}{label}");

        var childIndent = indent + (isLast ? "    " : "│   ");

        switch (expression)
        {
            case BinaryExpression binaryExpression:
                AppendExpression(lines, binaryExpression.Left, childIndent, false, options);
                AppendExpression(lines, binaryExpression.Right, childIndent, true, options);
                break;

            case UnaryExpression unaryExpression:
                AppendExpression(lines, unaryExpression.Operand, childIndent, true, options);
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

    private static string GetExpressionLabel(Expression expression, AstDumpOptions options) => expression switch
    {
        BinaryExpression binaryExpression => $"BinaryExpression [{MapOperator(binaryExpression.Operator)}]" + AppendLocationSuffix(options, expression),
        UnaryExpression unaryExpression => $"UnaryExpression [{MapUnaryOperator(unaryExpression.Operator)}]" + AppendLocationSuffix(options, expression),
        VariableExpression variableExpression => $"VariableExpression [{variableExpression.Name}]" + AppendLocationSuffix(options, expression),
        LiteralExpression literalExpression => GetLiteralLabel(literalExpression.Value) + AppendLocationSuffix(options, expression),
        _ => expression.GetType().Name + AppendLocationSuffix(options, expression)
    };

    private static string GetLiteralLabel(object value) => value switch
    {
        string text => $"StringLiteral [\"{EscapeString(text)}\"]",
        bool boolean => $"BooleanLiteral [{boolean.ToString().ToLowerInvariant()}]",
        double number => $"NumberLiteral [{FormatNumber(number)}]",
        float number => $"NumberLiteral [{FormatNumber(number)}]",
        decimal number => $"NumberLiteral [{FormatNumber(number)}]",
        int number => $"NumberLiteral [{number}]",
        long number => $"NumberLiteral [{number}]",
        _ => $"Literal [{value}]"
    };

    private static string AppendLocationSuffix(AstDumpOptions options, Expression expression)
    {
        if (!options.IncludeSourceLocations)
        {
            return string.Empty;
        }

        return " @ 0:0-0:0";
    }

    private static string MapOperator(string op) => op switch
    {
        "+" => "Add",
        "-" => "Subtract",
        "*" => "Multiply",
        "/" => "Divide",
        "==" => "Equal",
        "=" => "Equal",
        "!=" => "NotEqual",
        "<" => "Less",
        "<=" => "LessOrEqual",
        ">" => "Greater",
        ">=" => "GreaterOrEqual",
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

    private static string FormatNumber(double value)
    {
        if (Math.Abs(value - Math.Round(value)) < 0.0000001)
        {
            return Convert.ToInt64(value).ToString(CultureInfo.InvariantCulture);
        }

        return value.ToString("G", CultureInfo.InvariantCulture);
    }

    private static string FormatNumber(float value) => FormatNumber((double)value);

    private static string FormatNumber(decimal value)
    {
        if (value == decimal.Truncate(value))
        {
            return ((long)value).ToString(CultureInfo.InvariantCulture);
        }

        return value.ToString(CultureInfo.InvariantCulture);
    }

    private static string EscapeString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t")
            .Replace("\"", "\\\"");
    }
}
