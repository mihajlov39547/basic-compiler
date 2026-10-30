using BasicCompiler.Output;
using BasicCompiler.Semantics;

namespace BasicCompiler.Cli;

public sealed class InterpretCommand : CompilerCommandBase
{
    public override int Execute(string[] args)
    {
        var sourcePath = RequireSourcePath(args);

        var compiler = new MiniBasicCompiler();
        var compileResult = compiler.CompileFile(sourcePath);

        var lexerArtifactPath = ArtifactWriter.WriteLexerTokens(compileResult.Tokens, sourcePath);
        var astArtifactPath = ArtifactWriter.WriteAst(compileResult.Statements, sourcePath);

        var resolver = new NameResolver();
        resolver.Resolve(compileResult.Statements);
        var symbolArtifactPath = ArtifactWriter.WriteSymbols(resolver.GetSymbolDump(), sourcePath);

        var diagnosticsArtifactPath = ArtifactWriter.WriteDiagnostics(compileResult.Diagnostics, sourcePath);
        var runtimeArtifactPath = ArtifactWriter.WriteRuntimeResult(compileResult.RuntimeResult, sourcePath);

        Console.WriteLine($"MiniBASIC source: {sourcePath}");
        Console.WriteLine($"Lexer artifact saved to: {lexerArtifactPath}");
        Console.WriteLine($"AST artifact saved to: {astArtifactPath}");
        Console.WriteLine($"Symbols artifact saved to: {symbolArtifactPath}");
        Console.WriteLine($"Diagnostics artifact saved to: {diagnosticsArtifactPath}");
        Console.WriteLine($"Runtime artifact saved to: {runtimeArtifactPath}");
        Console.WriteLine($"\nExecution result: {compileResult.RuntimeResult}");

        if (compileResult.HasErrors)
        {
            Console.WriteLine("\nDiagnostics:");
            foreach (var diagnostic in compileResult.Diagnostics)
            {
                Console.WriteLine(diagnostic);
            }

            return 1;
        }

        return 0;
    }
}
