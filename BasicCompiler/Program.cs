using BasicCompiler.Cli;

if (args.Length == 0)
{
    Console.Error.WriteLine(CompilerCommandFactory.GetUsageText());
    return 1;
}

var factory = new CompilerCommandFactory();
var commandName = args[0];

if (commandName is "--help" or "-h" or "help")
{
    return factory.Create("help").Execute(Array.Empty<string>());
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

