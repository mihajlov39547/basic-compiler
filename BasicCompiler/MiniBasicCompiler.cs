using BasicCompiler.Diagnostics;
using BasicCompiler.Lexer;
using BasicCompiler.Runtime;
using BasicCompiler.Semantics;
using BasicCompiler.Syntax;
using BasicCompiler.Syntax.AST;
using LexerClass = BasicCompiler.Lexer.Lexer;

namespace BasicCompiler;

public sealed record CompilationResult(
    string Source,
    IReadOnlyList<Token> Tokens,
    IReadOnlyList<Statement> Statements,
    IReadOnlyList<Diagnostic> Diagnostics,
    object? RuntimeResult,
    bool Success)
{
    public bool HasErrors => Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error);
}

public sealed class MiniBasicCompiler
{
    public CompilationResult Compile(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentException("Source code cannot be empty.", nameof(source));
        }

        var tokens = new LexerClass(source).Tokenize();

        IReadOnlyList<Statement> statements;
        var diagnostics = new List<Diagnostic>();

        try
        {
            var parser = new Parser(tokens);
            statements = parser.ParseProgram();
        }
        catch (Exception ex)
        {
            return new CompilationResult(source, tokens, Array.Empty<Statement>(),
                [new Diagnostic(ex.Message, DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0))],
                null,
                false);
        }

        try
        {
            var resolver = new NameResolver();
            resolver.Resolve(statements);
        }
        catch (Exception ex)
        {
            diagnostics.Add(new Diagnostic(ex.Message, DiagnosticSeverity.Error, new SourceSpan(0, 0, 0, 0)));
        }

        diagnostics.AddRange(new TypeChecker().Check(statements));

        object? runtimeResult = null;
        if (!diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
        {
            runtimeResult = new Interpreter().Execute(statements);
        }

        return new CompilationResult(source, tokens, statements, diagnostics, runtimeResult, !diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error));
    }

    public CompilationResult CompileFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Source path cannot be empty.", nameof(path));
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Source file was not found.", path);
        }

        return Compile(File.ReadAllText(path));
    }
}
