using BasicCompiler.Lexer;

namespace BasicCompiler.Cli;

public sealed class LexCommand : CompilerCommandBase
{
    public override int Execute(string[] args)
    {
        var sourcePath = RequireSourcePath(args);
        var source = ReadSource(sourcePath);
        var tokens = new Lexer(source).Tokenize();
        var artifactPath = ArtifactWriter.WriteLexerTokens(tokens, sourcePath);

        Console.WriteLine($"MiniBASIC source: {sourcePath}");
        Console.WriteLine($"Lexer artifact saved to: {artifactPath}");
        Console.WriteLine("\nTokens:");

        foreach (var token in tokens)
        {
            if (token.Kind != TokenKind.EndOfFile)
            {
                Console.WriteLine($"{token.Kind}: {token.Text}");
            }
        }

        return 0;
    }
}
