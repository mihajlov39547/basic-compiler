using BasicCompiler.Diagnostics;
using BasicCompiler.Lexer;
using BasicCompiler.Output;
using BasicCompiler.Runtime;
using BasicCompiler.Syntax.AST;
using BasicCompiler.Text;

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

    [Fact]
    public void SampleProgram_ParsesAndExecutes()
    {
        var samplePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "sample.bas"));
        var source = File.ReadAllText(samplePath);

        var tokens = new BasicCompiler.Lexer.Lexer(source).Tokenize();
        var statements = new BasicCompiler.Syntax.Parser(tokens).ParseProgram();
        var result = new BasicCompiler.Runtime.Interpreter().Execute(statements);

        Assert.NotNull(result);
    }

    [Fact]
    public void RuntimeValue_UsesTypedValuesForNumberStringAndBoolean()
    {
        var number = RuntimeValue.Number(42);
        var text = RuntimeValue.String("Ada");
        var flag = RuntimeValue.Boolean(true);

        Assert.Equal("number", number.Kind);
        Assert.Equal("string", text.Kind);
        Assert.Equal("boolean", flag.Kind);
        Assert.Equal(42d, ((NumberValue)number).Value, 0.0001);
        Assert.Equal("Ada", ((StringValue)text).Value);
        Assert.True(((BooleanValue)flag).Value);
    }

    [Fact]
    public void Interpreter_EvaluatesArithmeticAndBooleanExpressions()
    {
        var statements = new Statement[]
        {
            new VariableDeclaration("a", new LiteralExpression(10.0)),
            new VariableDeclaration("b", new LiteralExpression(5.0)),
            new VariableDeclaration("ok", new BinaryExpression(new VariableExpression("a"), ">", new VariableExpression("b")))
        };

        var result = new BasicCompiler.Runtime.Interpreter().Execute(statements);

        Assert.True((bool)result!);
        Assert.Equal(true, result);
    }

    [Fact]
    public void SourceText_TracksLinesAndColumns()
    {
        var sourceText = SourceText.FromText("abc\nxy\nz");

        Assert.Equal(3, sourceText.LineCount);
        Assert.Equal(0, sourceText.GetLineStart(0));
        Assert.Equal(4, sourceText.GetLineStart(1));
        Assert.Equal(7, sourceText.GetLineStart(2));

        var position = sourceText.GetPosition(5);
        Assert.Equal(1, position.Line);
        Assert.Equal(1, position.Column);

        var span = sourceText.GetSpan(4, 2);
        Assert.Equal(1, span.Line);
        Assert.Equal(0, span.Column);
        Assert.Equal(2, span.Length);
    }

    [Fact]
    public void DiagnosticBag_CollectsAndFormatsMultipleDiagnostics()
    {
        var bag = new BasicCompiler.Diagnostics.DiagnosticBag();
        bag.ReportError("Missing semicolon.", new SourceSpan(4, 1, 1, 5));
        bag.ReportWarning("Expression is a little unusual.", new SourceSpan(10, 1, 2, 1));

        Assert.Equal(2, bag.Diagnostics.Count);
        Assert.Equal(DiagnosticSeverity.Error, bag.Diagnostics[0].Severity);
        Assert.Contains("Missing semicolon", bag.ToString());
        Assert.Contains("Warning", bag.ToString());
    }

    [Fact]
    public void NameResolver_ResolvesVariablesAcrossNestedBlocks()
    {
        var statements = new Statement[]
        {
            new VariableDeclaration("outer", new LiteralExpression(1.0)),
            new IfStatement(
                new LiteralExpression(true),
                new BlockStatement(new Statement[]
                {
                    new VariableDeclaration("inner", new BinaryExpression(new VariableExpression("outer"), "+", new LiteralExpression(2.0))),
                    new PrintStatement(new VariableExpression("inner"))
                }),
                null)
        };

        var resolver = new BasicCompiler.Semantics.NameResolver();
        resolver.Resolve(statements);

        var dump = resolver.GetSymbolDump();
        Assert.Contains("outer", dump);
        Assert.Contains("inner", dump);
    }

    [Fact]
    public void NameResolver_ReportsDuplicateNamesInSameScope()
    {
        var statements = new Statement[]
        {
            new VariableDeclaration("value", new LiteralExpression(10.0)),
            new VariableDeclaration("value", new LiteralExpression(20.0))
        };

        var resolver = new BasicCompiler.Semantics.NameResolver();

        var exception = Assert.Throws<InvalidOperationException>(() => resolver.Resolve(statements));
        Assert.Contains("already declared", exception.Message);
    }

    [Fact]
    public void VariableSymbol_IsCreatedWithNameAndType()
    {
        var symbol = new BasicCompiler.Semantics.VariableSymbol("total", "number");

        Assert.Equal("total", symbol.Name);
        Assert.Equal("number", symbol.TypeName);
        Assert.True(symbol.IsMutable);
    }

    [Fact]
    public void TypeChecker_RejectsNonBooleanIfCondition()
    {
        var statements = new Statement[]
        {
            new IfStatement(new LiteralExpression("hello"), new PrintStatement(new LiteralExpression("done")), null)
        };

        var diagnostics = new BasicCompiler.Semantics.TypeChecker().Check(statements);

        Assert.Contains(diagnostics, d => d.Message.Contains("bool", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void TypeChecker_RejectsInvalidArithmeticTypes()
    {
        var statements = new Statement[]
        {
            new PrintStatement(new BinaryExpression(new LiteralExpression("hello"), "+", new LiteralExpression(1.0)))
        };

        var diagnostics = new BasicCompiler.Semantics.TypeChecker().Check(statements);

        Assert.Contains(diagnostics, d => d.Message.Contains("numeric", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void TypeChecker_RejectsAssignmentTypeMismatch()
    {
        var statements = new Statement[]
        {
            new VariableDeclaration("count", new LiteralExpression(1.0)),
            new AssignmentStatement("count", new LiteralExpression(true))
        };

        var diagnostics = new BasicCompiler.Semantics.TypeChecker().Check(statements);

        Assert.Contains(diagnostics, d => d.Message.Contains("assignment", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void LexerArtifact_IsWrittenToOutputDirectory()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("var total := 42").Tokenize();
        var artifactPath = ArtifactWriter.WriteLexerTokens(tokens, "demo.bas");

        Assert.True(File.Exists(artifactPath));
        Assert.Contains("Var", File.ReadAllText(artifactPath));
        Assert.Contains("#", File.ReadAllText(artifactPath));
        Assert.Contains("Value", File.ReadAllText(artifactPath));
        Assert.Contains("Span", File.ReadAllText(artifactPath));
        Assert.Contains("Location", File.ReadAllText(artifactPath));
        Assert.Contains("EOF", File.ReadAllText(artifactPath));
        Assert.Equal("01-tokens.txt", Path.GetFileName(artifactPath));
    }

    [Fact]
    public void AstArtifact_IsWrittenToOutputDirectory()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("var total := 42\nprint(total)").Tokenize();
        var statements = new BasicCompiler.Syntax.Parser(tokens).ParseProgram();
        var artifactPath = ArtifactWriter.WriteAst(statements, "demo.bas");

        Assert.True(File.Exists(artifactPath));
        Assert.Contains("Program", File.ReadAllText(artifactPath));
        Assert.Contains("VariableDeclaration", File.ReadAllText(artifactPath));
        Assert.Equal("02-ast.txt", Path.GetFileName(artifactPath));
    }

    [Fact]
    public void AstPrinter_GoldenOutput_ForOperatorPrecedence()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("var a := 1 + 2 * 3").Tokenize();
        var statements = new BasicCompiler.Syntax.Parser(tokens).ParseProgram();
        var dump = AstPrinter.Print(statements);

        var expected = string.Join(System.Environment.NewLine,
            "Program",
            "└── VariableDeclaration",
            "    ├── Name: a",
            "    └── Initializer",
            "        └── BinaryExpression [Add]",
            "            ├── NumberLiteral [1]",
            "            └── BinaryExpression [Multiply]",
            "                ├── NumberLiteral [2]",
            "                └── NumberLiteral [3]");

        Assert.Equal(expected, dump);
    }

    [Fact]
    public void AstPrinter_GoldenOutput_ForParenthesizedExpression()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("var a := (1 + 2) * 3").Tokenize();
        var statements = new BasicCompiler.Syntax.Parser(tokens).ParseProgram();
        var dump = AstPrinter.Print(statements);

        Assert.Contains("BinaryExpression [Multiply]", dump);
        Assert.Contains("BinaryExpression [Add]", dump);
        Assert.True(dump.IndexOf("BinaryExpression [Multiply]", StringComparison.Ordinal) < dump.IndexOf("BinaryExpression [Add]", StringComparison.Ordinal));
    }

    [Fact]
    public void AstPrinter_UsesStructuredIfElseLabels()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("if a = 1 then\n print(\"yes\")\nelse\n print(\"no\")\nendif").Tokenize();
        var statements = new BasicCompiler.Syntax.Parser(tokens).ParseProgram();
        var dump = AstPrinter.Print(statements);

        Assert.Contains("IfStatement", dump);
        Assert.Contains("Condition", dump);
        Assert.Contains("Then", dump);
        Assert.Contains("Else", dump);
        Assert.Contains("StringLiteral [\"yes\"]", dump);
    }

    [Fact]
    public void AstPrinter_ShowsLogicalPrecedence()
    {
        var tokens = new BasicCompiler.Lexer.Lexer("if a = 1 || b = 2 && c = 3 then\n print(\"yes\")\nendif").Tokenize();
        var statements = new BasicCompiler.Syntax.Parser(tokens).ParseProgram();
        var dump = AstPrinter.Print(statements);

        Assert.Contains("BinaryExpression [LogicalOr]", dump);
        Assert.Contains("BinaryExpression [LogicalAnd]", dump);
        Assert.True(dump.IndexOf("BinaryExpression [LogicalOr]", StringComparison.Ordinal) < dump.IndexOf("BinaryExpression [LogicalAnd]", StringComparison.Ordinal));
    }
}
