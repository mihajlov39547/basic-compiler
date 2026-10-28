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
        if (Match(TokenKind.Let))
        {
            return ParseVariableDeclaration();
        }

        if (Match(TokenKind.If))
        {
            return ParseIfStatement();
        }

        if (Match(TokenKind.Print))
        {
            var expression = ParseExpression();
            Consume(TokenKind.Semicolon, "Expected ';' after print statement.");
            return new PrintStatement(expression);
        }

        throw new InvalidOperationException($"Unexpected token '{Current.Text}' while parsing statement.");
    }

    private VariableDeclaration ParseVariableDeclaration()
    {
        var name = Consume(TokenKind.Identifier, "Expected variable name.").Text;

        Expression? initializer = null;
        if (Match(TokenKind.Equals))
        {
            initializer = ParseExpression();
        }

        Consume(TokenKind.Semicolon, "Expected ';' after variable declaration.");
        return new VariableDeclaration(name, initializer);
    }

    private IfStatement ParseIfStatement()
    {
        var condition = ParseExpression();
        Consume(TokenKind.Then, "Expected 'then' after if condition.");

        var thenStatement = ParseStatement();
        Statement? elseStatement = null;

        if (Match(TokenKind.Else))
        {
            elseStatement = ParseStatement();
        }

        return new IfStatement(condition, thenStatement, elseStatement);
    }

    private Expression ParseExpression() => ParseComparison();

    private Expression ParseComparison()
    {
        var expression = ParseAdditive();

        while (Current.Kind is TokenKind.EqualsEquals or TokenKind.NotEquals or TokenKind.LessThan or TokenKind.GreaterThan or TokenKind.LessThanOrEqual or TokenKind.GreaterThanOrEqual)
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
        var expression = ParsePrimary();

        while (Current.Kind is TokenKind.Star or TokenKind.Slash)
        {
            var op = Advance().Text;
            var right = ParsePrimary();
            expression = new BinaryExpression(expression, op, right);
        }

        return expression;
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
    private bool IsAtEnd() => Current.Kind == TokenKind.EndOfFile;
}
