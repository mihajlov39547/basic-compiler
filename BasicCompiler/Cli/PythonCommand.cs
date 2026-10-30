using BasicCompiler.CodeGeneration;
using BasicCompiler.Lexer;
using BasicCompiler.Output;
using BasicCompiler.Syntax;
using LexerClass = BasicCompiler.Lexer.Lexer;

namespace BasicCompiler.Cli;

public sealed class PythonCommand : CompilerCommandBase
{
    public override int Execute(string[] args)
    {
        var sourcePath = RequireSourcePath(args);
        var source = ReadSource(sourcePath);
        var tokens = new LexerClass(source).Tokenize();
        var statements = new Parser(tokens).ParseProgram();
        var pythonCode = new PythonCodeGenerator().Generate(statements);
        var artifactPath = ArtifactWriter.WritePythonCode(pythonCode, sourcePath);

        Console.WriteLine($"MiniBASIC source: {sourcePath}");
        Console.WriteLine($"Python code saved to: {artifactPath}");
        Console.WriteLine();
        Console.WriteLine(pythonCode);

        return 0;
    }
}
