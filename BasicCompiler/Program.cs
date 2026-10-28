using BasicCompiler.Cli;

var factory = new CompilerCommandFactory();
var defaultSamplePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "sample.bas"));

if (args.Length == 0)
{
    if (File.Exists(defaultSamplePath))
    {
        return factory.Create("interpret").Execute(new[] { defaultSamplePath });
    }

    Console.Error.WriteLine(CompilerCommandFactory.GetUsageText());
    return 1;
}

var commandName = args[0];

if (commandName is "--help" or "-h" or "help")
{
    return factory.Create("help").Execute(Array.Empty<string>());
}

if (commandName is "--sample" or "sample")
{
    if (File.Exists(defaultSamplePath))
    {
        return factory.Create("interpret").Execute(new[] { defaultSamplePath });
    }

    Console.Error.WriteLine("The bundled sample.bas file was not found.");
    return 1;
}

if (factory.IsKnownCommand(commandName))
{
    return factory.Create(commandName).Execute(args[1..]);
}

if (args.Length == 1 && File.Exists(commandName))
{
    return factory.Create("interpret").Execute(args);
}

Console.Error.WriteLine($"Unknown command '{commandName}'.");
Console.Error.WriteLine(CompilerCommandFactory.GetUsageText());
return 1;

