using BasicCompiler.Lexer;
using BasicCompiler.Semantics;
using BasicCompiler.Syntax;

namespace BasicCompiler.Cli;

public sealed class CheckCommand : CompilerCommandBase
{
    public override int Execute(string[] args)
    {
        var sourcePath = RequireSourcePath(args);
        var source = ReadSource(sourcePath);
        var tokens = new Lexer(source).Tokenize();
        var statements = new Parser(tokens).ParseProgram();
        var resolver = new NameResolver();
        resolver.Resolve(statements);

        Console.WriteLine($"Checking source: {sourcePath}");
        Console.WriteLine("\nSymbols:");
        Console.WriteLine(resolver.GetSymbolDump());

        var diagnostics = new TypeChecker().Check(statements);
        if (diagnostics.Count == 0)
        {
            Console.WriteLine("\nNo type or name diagnostics.");
            return 0;
        }

        Console.WriteLine("\nDiagnostics:");
        foreach (var diagnostic in diagnostics)
        {
            Console.WriteLine(diagnostic);
        }

        return 1;
    }
}
