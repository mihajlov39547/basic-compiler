namespace BasicCompiler.Cli;

internal static class CliParser
{
    public static ParsedCommand Parse(string[] args, CompilerCommandFactory? factory = null)
    {
        factory ??= new CompilerCommandFactory();

        if (args.Length == 0)
        {
            return new ParsedCommand();
        }

        var command = default(string);
        var commandArgs = new List<string>();
        var positionalSourcePath = default(string);
        var showHelp = false;
        var runSample = false;

        for (var i = 0; i < args.Length; i++)
        {
            var token = args[i];

            switch (token)
            {
                case "-h":
                case "--help":
                    showHelp = true;
                    break;

                case "--sample":
                    runSample = true;
                    break;

                case "--file":
                    if (i + 1 >= args.Length)
                    {
                        throw new InvalidOperationException("The --file option requires a path.");
                    }

                    positionalSourcePath = args[++i];
                    break;

                default:
                    if (command is null)
                    {
                        if (factory.IsKnownCommand(token))
                        {
                            command = token;
                            continue;
                        }

                        if (File.Exists(token))
                        {
                            positionalSourcePath = token;
                            continue;
                        }

                        if (token.StartsWith("-", StringComparison.Ordinal))
                        {
                            throw new InvalidOperationException($"Unknown option '{token}'.");
                        }

                        throw new InvalidOperationException($"Unknown command or file '{token}'.");
                    }

                    commandArgs.Add(token);
                    break;
            }
        }

        if (command is not null)
        {
            var normalizedArgs = NormalizeCommandArgs(commandArgs);
            return new ParsedCommand(command, normalizedArgs, null, showHelp, runSample);
        }

        if (positionalSourcePath is not null)
        {
            return new ParsedCommand(null, Array.Empty<string>(), positionalSourcePath, showHelp, runSample);
        }

        return new ParsedCommand(null, Array.Empty<string>(), null, showHelp, runSample);
    }

    private static string[] NormalizeCommandArgs(IEnumerable<string> args)
    {
        var result = new List<string>();

        foreach (var arg in args)
        {
            if (arg == "--file")
            {
                continue;
            }

            if ((arg == "-h" || arg == "--help") && result.Count == 0)
            {
                continue;
            }

            if (arg.StartsWith("--", StringComparison.Ordinal) && arg != "--")
            {
                throw new InvalidOperationException($"Unsupported option for command: {arg}");
            }

            result.Add(arg);
        }

        return result.ToArray();
    }
}
