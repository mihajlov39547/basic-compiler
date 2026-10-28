using BasicCompiler.Lexer;

namespace BasicCompiler.Tests;

public class LexerTests
{
    [Fact]
    public void SingleKeyword_IsTokenized()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("if").Tokenize();

        Assert.Equal(new[] { TokenKind.If, TokenKind.EndOfFile }, tokens.Select(t => t.Kind).ToArray());
    }

    [Fact]
    public void Identifier_IsTokenized()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("message").Tokenize();

        Assert.Equal(new[] { TokenKind.Identifier, TokenKind.EndOfFile }, tokens.Select(t => t.Kind).ToArray());
        Assert.Equal("message", tokens[0].Text);
    }

    [Fact]
    public void IntegerNumber_IsTokenized()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("42").Tokenize();

        Assert.Equal(new[] { TokenKind.Number, TokenKind.EndOfFile }, tokens.Select(t => t.Kind).ToArray());
        Assert.Equal("42", tokens[0].Text);
    }

    [Fact]
    public void DecimalNumber_IsTokenized()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("42.5").Tokenize();

        Assert.Equal(new[] { TokenKind.Number, TokenKind.EndOfFile }, tokens.Select(t => t.Kind).ToArray());
        Assert.Equal("42.5", tokens[0].Text);
    }

    [Fact]
    public void StringLiteral_IsTokenized()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("\"hello\"").Tokenize();

        Assert.Equal(new[] { TokenKind.String, TokenKind.EndOfFile }, tokens.Select(t => t.Kind).ToArray());
        Assert.Equal("\"hello\"", tokens[0].Text);
    }

    [Fact]
    public void VariableDeclaration_IsTokenized()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("var answer := 42").Tokenize();

        Assert.Equal(
            new[]
            {
                TokenKind.Var,
                TokenKind.Identifier,
                TokenKind.Assign,
                TokenKind.Number,
                TokenKind.EndOfFile
            },
            tokens.Select(t => t.Kind).ToArray());
    }

    [Fact]
    public void Operators_AreTokenized()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("a + b * c / d = != < <= > >= && || !").Tokenize();

        Assert.Contains(tokens, t => t.Kind == TokenKind.Plus);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Star);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Slash);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Equals);
        Assert.Contains(tokens, t => t.Kind == TokenKind.NotEquals);
        Assert.Contains(tokens, t => t.Kind == TokenKind.LessThan);
        Assert.Contains(tokens, t => t.Kind == TokenKind.LessThanOrEqual);
        Assert.Contains(tokens, t => t.Kind == TokenKind.GreaterThan);
        Assert.Contains(tokens, t => t.Kind == TokenKind.GreaterThanOrEqual);
        Assert.Contains(tokens, t => t.Kind == TokenKind.AndAnd);
        Assert.Contains(tokens, t => t.Kind == TokenKind.OrOr);
        Assert.Contains(tokens, t => t.Kind == TokenKind.Not);
    }

    [Fact]
    public void Whitespace_IsIgnored()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("  var   answer  :=  42\n\n").Tokenize();

        Assert.Equal(
            new[]
            {
                TokenKind.Var,
                TokenKind.Identifier,
                TokenKind.Assign,
                TokenKind.Number,
                TokenKind.EndOfFile
            },
            tokens.Select(t => t.Kind).ToArray());
    }

    [Fact]
    public void MultipleLines_AreHandled()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("var a := 1\nvar b := 2\nprint(a + b)").Tokenize();

        Assert.Equal(TokenKind.Var, tokens[0].Kind);
        Assert.Equal(TokenKind.Identifier, tokens[1].Kind);
        Assert.Equal(TokenKind.Assign, tokens[2].Kind);
        Assert.Equal(TokenKind.Number, tokens[3].Kind);
        Assert.Equal(TokenKind.Var, tokens[4].Kind);
    }

    [Fact]
    public void SourcePositions_AreTracked()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("var answer := 42").Tokenize();

        Assert.Equal(0, tokens[0].Position);
        Assert.Equal(4, tokens[1].Position);
        Assert.Equal(11, tokens[2].Position);
        Assert.Equal(14, tokens[3].Position);
    }

    [Fact]
    public void UnknownCharacter_ThrowsDiagnosticLikeFailure()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => new BasicCompiler.Lexer.Lexer("var x @ 1").Tokenize());
        Assert.Contains("@", exception.Message);
    }

    [Fact]
    public void UnterminatedString_ThrowsDiagnosticLikeFailure()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => new BasicCompiler.Lexer.Lexer("\"unterminated").Tokenize());
        Assert.Contains("Unterminated string", exception.Message);
    }
}
