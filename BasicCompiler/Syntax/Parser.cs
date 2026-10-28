using BasicCompiler.Lexer;
using BasicCompiler.Syntax.AST;

namespace BasicCompiler.Syntax;

public sealed class Parser
{
    private readonly IReadOnlyList<Token> _tokens;
    private int _index;

    public Parser(IReadOnlyList<Token> tokens)
    {
        _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
    }

    public IReadOnlyList<Statement> ParseProgram()
    {
        var statements = new List<Statement>();

        while (!IsAtEnd())
        {
            if (Current.Kind == TokenKind.EndOfFile)
            {
                break;
            }

            statements.Add(ParseStatement());
        }

        return statements;
    }

    private Statement ParseStatement()
    {
        if (Match(TokenKind.Var))
        {
            return ParseVariableDeclaration();
        }

        if (Match(TokenKind.If))
        {
            return ParseIfStatement();
        }

        if (Match(TokenKind.While))
        {
            return ParseWhileStatement();
        }

        if (Match(TokenKind.Print))
        {
            return ParsePrintStatement();
        }

        if (Current.Kind == TokenKind.Identifier && Peek(1).Kind == TokenKind.Assign)
        {
            return ParseAssignmentStatement();
        }

        throw new InvalidOperationException($"Unexpected token '{Current.Text}' while parsing statement.");
    }

    private VariableDeclaration ParseVariableDeclaration()
    {
        var name = Consume(TokenKind.Identifier, "Expected variable name.").Text;
        Consume(TokenKind.Assign, "Expected ':=' after variable name.");
        var initializer = ParseExpression();
        ConsumeOptionalSemicolon();
        return new VariableDeclaration(name, initializer);
    }

    private AssignmentStatement ParseAssignmentStatement()
    {
        var name = Consume(TokenKind.Identifier, "Expected variable name.").Text;
        Consume(TokenKind.Assign, "Expected ':=' after variable name.");
        var expression = ParseExpression();
        ConsumeOptionalSemicolon();
        return new AssignmentStatement(name, expression);
    }

    private PrintStatement ParsePrintStatement()
    {
        var expression = Match(TokenKind.LParen)
            ? ParseExpression()
            : ParseExpression();

        if (Previous().Kind != TokenKind.RParen && Current.Kind == TokenKind.RParen)
        {
            Consume(TokenKind.RParen, "Expected ')' after expression.");
        }

        ConsumeOptionalSemicolon();
        return new PrintStatement(expression);
    }

    private IfStatement ParseIfStatement()
    {
        var condition = ParseExpression();
        Consume(TokenKind.Then, "Expected 'then' after if condition.");

        var thenBlock = ParseBlockUntil(TokenKind.Else, TokenKind.EndIf);
        Statement? elseBlock = null;

        if (Match(TokenKind.Else))
        {
            elseBlock = ParseBlockUntil(TokenKind.EndIf);
        }

        Consume(TokenKind.EndIf, "Expected 'endif' after if block.");
        return new IfStatement(condition, thenBlock, elseBlock);
    }

    private WhileStatement ParseWhileStatement()
    {
        var condition = ParseExpression();
        Consume(TokenKind.Do, "Expected 'do' after while condition.");
        var body = ParseBlockUntil(TokenKind.EndWhile);
        Consume(TokenKind.EndWhile, "Expected 'endwhile' after while block.");
        return new WhileStatement(condition, body);
    }

    private BlockStatement ParseBlockUntil(params TokenKind[] terminators)
    {
        var statements = new List<Statement>();

        while (!IsAtEnd() && !terminators.Contains(Current.Kind))
        {
            statements.Add(ParseStatement());
        }

        return new BlockStatement(statements);
    }

    private Expression ParseExpression() => ParseLogicalOr();

    private Expression ParseLogicalOr()
    {
        var expression = ParseLogicalAnd();

        while (Match(TokenKind.OrOr))
        {
            var op = Previous().Text;
            var right = ParseLogicalAnd();
            expression = new BinaryExpression(expression, op, right);
        }

        return expression;
    }

    private Expression ParseLogicalAnd()
    {
        var expression = ParseEquality();

        while (Match(TokenKind.AndAnd))
        {
            var op = Previous().Text;
            var right = ParseEquality();
            expression = new BinaryExpression(expression, op, right);
        }

        return expression;
    }

    private Expression ParseEquality()
    {
        var expression = ParseComparison();

        while (Current.Kind is TokenKind.Equals or TokenKind.EqualsEquals or TokenKind.NotEquals)
        {
            var op = Advance().Text;
            var right = ParseComparison();
            expression = new BinaryExpression(expression, op, right);
        }

        return expression;
    }

    private Expression ParseComparison()
    {
        var expression = ParseAdditive();

        while (Current.Kind is TokenKind.LessThan or TokenKind.GreaterThan or TokenKind.LessThanOrEqual or TokenKind.GreaterThanOrEqual)
        {
            var op = Advance().Text;
            var right = ParseAdditive();
            expression = new BinaryExpression(expression, op, right);
        }

        return expression;
    }

    private Expression ParseAdditive()
    {
        var expression = ParseMultiplicative();

        while (Current.Kind is TokenKind.Plus or TokenKind.Minus)
        {
            var op = Advance().Text;
            var right = ParseMultiplicative();
            expression = new BinaryExpression(expression, op, right);
        }

        return expression;
    }

    private Expression ParseMultiplicative()
    {
        var expression = ParseUnary();

        while (Current.Kind is TokenKind.Star or TokenKind.Slash)
        {
            var op = Advance().Text;
            var right = ParseUnary();
            expression = new BinaryExpression(expression, op, right);
        }

        return expression;
    }

    private Expression ParseUnary()
    {
        if (Match(TokenKind.Not))
        {
            var op = Previous().Text;
            return new UnaryExpression(op, ParseUnary());
        }

        if (Match(TokenKind.Minus))
        {
            var op = Previous().Text;
            return new UnaryExpression(op, ParseUnary());
        }

        return ParsePrimary();
    }

    private Expression ParsePrimary()
    {
        if (Match(TokenKind.Number))
        {
            return new LiteralExpression(double.Parse(Previous().Text));
        }

        if (Match(TokenKind.True))
        {
            return new LiteralExpression(true);
        }

        if (Match(TokenKind.False))
        {
            return new LiteralExpression(false);
        }

        if (Match(TokenKind.String))
        {
            var text = Previous().Text;
            return new LiteralExpression(UnescapeStringLiteral(text));
        }

        if (Match(TokenKind.Identifier))
        {
            return new VariableExpression(Previous().Text);
        }

        if (Match(TokenKind.LParen))
        {
            var inner = ParseExpression();
            Consume(TokenKind.RParen, "Expected ')' after expression.");
            return inner;
        }

        throw new InvalidOperationException($"Unexpected token '{Current.Text}' while parsing expression.");
    }

    private static string UnescapeStringLiteral(string literal)
    {
        if (literal.Length < 2)
        {
            return literal;
        }

        var inner = literal.Substring(1, literal.Length - 2);
        var builder = new System.Text.StringBuilder();

        for (var i = 0; i < inner.Length; i++)
        {
            if (inner[i] == '\\' && i + 1 < inner.Length)
            {
                builder.Append(inner[i + 1] switch
                {
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    '\\' => '\\',
                    '"' => '"',
                    _ => inner[i + 1]
                });
                i++;
                continue;
            }

            builder.Append(inner[i]);
        }

        return builder.ToString();
    }

    private void ConsumeOptionalSemicolon()
    {
        if (Current.Kind == TokenKind.Semicolon)
        {
            Advance();
        }
    }

    private bool Match(params TokenKind[] kinds)
    {
        if (kinds.Contains(Current.Kind))
        {
            _index++;
            return true;
        }

        return false;
    }

    private Token Consume(TokenKind kind, string message)
    {
        if (Current.Kind == kind)
        {
            return Advance();
        }

        throw new InvalidOperationException(message);
    }

    private Token Advance()
    {
        if (!IsAtEnd())
        {
            _index++;
        }

        return Previous();
    }

    private Token Current => _tokens[_index];
    private Token Previous() => _tokens[_index - 1];
    private Token Peek(int offset)
    {
        var index = _index + offset;
        if (index >= _tokens.Count)
        {
            return _tokens[^1];
        }

        return _tokens[index];
    }

    private bool IsAtEnd() => Current.Kind == TokenKind.EndOfFile;
}
