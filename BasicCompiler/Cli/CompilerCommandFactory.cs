namespace BasicCompiler.Cli;

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
