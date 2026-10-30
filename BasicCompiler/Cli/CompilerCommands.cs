using BasicCompiler.Diagnostics;
using BasicCompiler.Lexer;
using BasicCompiler.Output;
using BasicCompiler.Runtime;
using BasicCompiler.Semantics;
using BasicCompiler.Syntax;
using BasicCompiler.Syntax.AST;
using LexerClass = BasicCompiler.Lexer.Lexer;

namespace BasicCompiler.Cli;

public interface ICompilerCommand
{
    int Execute(string[] args);
}

public sealed class CompilerCommandFactory
{
    private readonly Dictionary<string, Func<ICompilerCommand>> _commands = new(StringComparer.OrdinalIgnoreCase)
    {
        ["lex"] = () => new LexCommand(),
        ["parse"] = () => new ParseCommand(),
        ["check"] = () => new CheckCommand(),
        ["interpret"] = () => new InterpretCommand(),
        ["help"] = () => new HelpCommand()
    };

    public IReadOnlyCollection<string> GetAvailableCommands() => _commands.Keys.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToArray();

    public bool IsKnownCommand(string commandName) => _commands.ContainsKey(commandName);

    public ICompilerCommand Create(string commandName)
    {
        if (_commands.TryGetValue(commandName, out var factory))
        {
            return factory();
        }

        throw new InvalidOperationException($"Unknown command '{commandName}'. Available commands: {string.Join(", ", GetAvailableCommands())}.");
    }

    public static string GetUsageText()
    {
        return "Usage: dotnet run --project BasicCompiler.csproj -- [command] [options]\n" +
               "       dotnet run --project BasicCompiler.csproj -- lex sample.bas\n" +
               "       dotnet run --project BasicCompiler.csproj -- interpret --file sample.bas\n" +
               "       dotnet run --project BasicCompiler.csproj -- --sample\n" +
               "       dotnet run --project BasicCompiler.csproj --  # defaults to the bundled sample.bas\n" +
               "\nCommands:\n" +
               "  lex       tokenize a source file\n" +
               "  parse     parse a source file into an AST\n" +
               "  check     resolve names and validate types\n" +
               "  interpret run the program with the reference interpreter\n" +
               "  help      show this help text\n" +
               "\nOptions:\n" +
               "  --file PATH    source file to use with a command\n" +
               "  --sample       run the bundled sample.bas\n" +
               "  -h, --help     show this help text\n" +
               "\nExamples:\n" +
               "  dotnet run --project BasicCompiler.csproj -- lex --file sample.bas\n" +
               "  dotnet run --project BasicCompiler.csproj -- parse sample.bas\n" +
               "  dotnet run --project BasicCompiler.csproj -- check --file sample.bas\n" +
               "  dotnet run --project BasicCompiler.csproj -- interpret --file sample.bas\n" +
               "  dotnet run --project BasicCompiler.csproj -- --sample";
    }
}

public abstract class CompilerCommandBase : ICompilerCommand
{
    public abstract int Execute(string[] args);

    protected static string RequireSourcePath(string[] args)
    {
        if (args.Length != 1)
        {
            throw new InvalidOperationException($"This command requires exactly one source file.\n{CompilerCommandFactory.GetUsageText()}");
        }

        var sourcePath = args[0];
        if (!File.Exists(sourcePath))
        {
            throw new InvalidOperationException($"File not found: {sourcePath}");
        }

        return sourcePath;
    }

    protected static string ReadSource(string sourcePath) => File.ReadAllText(sourcePath);
}

public sealed class HelpCommand : CompilerCommandBase
{
    public override int Execute(string[] args)
    {
        Console.WriteLine(CompilerCommandFactory.GetUsageText());
        return 0;
    }
}

public sealed class LexCommand : CompilerCommandBase
{
    public override int Execute(string[] args)
    {
        var sourcePath = RequireSourcePath(args);
        var source = ReadSource(sourcePath);
        var tokens = new LexerClass(source).Tokenize();
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

public sealed class ParseCommand : CompilerCommandBase
{
    public override int Execute(string[] args)
    {
        var sourcePath = RequireSourcePath(args);
        var source = ReadSource(sourcePath);
        var tokens = new LexerClass(source).Tokenize();
        var statements = new Parser(tokens).ParseProgram();
        var artifactPath = ArtifactWriter.WriteAst(statements, sourcePath);

        Console.WriteLine($"MiniBASIC source: {sourcePath}");
        Console.WriteLine($"AST artifact saved to: {artifactPath}");
        Console.WriteLine("\nAST:");
        Console.WriteLine(AstPrinter.Print(statements));

        return 0;
    }
}

public sealed class CheckCommand : CompilerCommandBase
{
    public override int Execute(string[] args)
    {
        var sourcePath = RequireSourcePath(args);
        var source = ReadSource(sourcePath);
        var tokens = new LexerClass(source).Tokenize();
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
