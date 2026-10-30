using BasicCompiler.Lexer;
using BasicCompiler.Syntax;

namespace BasicCompiler.Cli;

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
