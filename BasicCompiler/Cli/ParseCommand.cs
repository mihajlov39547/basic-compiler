using BasicCompiler.Lexer;
using BasicCompiler.Syntax;

namespace BasicCompiler.Cli;

public sealed class ParseCommand : CompilerCommandBase
{
    public override int Execute(string[] args)
    {
        var sourcePath = RequireSourcePath(args);
        var source = ReadSource(sourcePath);
        var tokens = new Lexer(source).Tokenize();
        var statements = new Parser(tokens).ParseProgram();
        var artifactPath = ArtifactWriter.WriteAst(statements, sourcePath);

        Console.WriteLine($"MiniBASIC source: {sourcePath}");
        Console.WriteLine($"AST artifact saved to: {artifactPath}");
        Console.WriteLine("\nAST:");
        Console.WriteLine(AstPrinter.Print(statements));

        return 0;
    }
}
