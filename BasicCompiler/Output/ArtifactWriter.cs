using BasicCompiler.Lexer;
using BasicCompiler.Syntax.AST;

namespace BasicCompiler.Output;

public static class ArtifactWriter
{
    public static string WriteLexerTokens(IReadOnlyList<Token> tokens, string sourcePath)
    {
        var outputDirectory = GetOutputDirectory();
        var filePath = Path.Combine(outputDirectory, "01-tokens.txt");

        var lines = new List<string>
        {
            $"Source: {Path.GetFileName(sourcePath)}",
            $"Generated: {DateTime.UtcNow:O}",
            $"Token count: {tokens.Count(t => t.Kind != TokenKind.EndOfFile)}",
            string.Empty
        };

        foreach (var token in tokens)
        {
            if (token.Kind == TokenKind.EndOfFile)
            {
                continue;
            }

            lines.Add($"{token.Kind,-16} | {token.Text,-18} | position={token.Position}");
        }

        Directory.CreateDirectory(outputDirectory);
        File.WriteAllLines(filePath, lines);
        return filePath;
    }

    public static string WriteAst(IReadOnlyList<Statement> statements, string sourcePath)
    {
        var outputDirectory = GetOutputDirectory();
        var filePath = Path.Combine(outputDirectory, "02-ast.txt");

        var lines = new List<string>
        {
            $"Source: {Path.GetFileName(sourcePath)}",
            $"Generated: {DateTime.UtcNow:O}",
            $"Statement count: {statements.Count}",
            string.Empty,
            AstPrinter.Print(statements)
        };

        Directory.CreateDirectory(outputDirectory);
        File.WriteAllLines(filePath, lines);
        return filePath;
    }

    public static string GetOutputDirectory()
    {
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var outputDirectory = Path.Combine(projectRoot, "output");
        return outputDirectory;
    }
}
