using BasicCompiler.Cli;

return ProgramEntry.Run(args);

internal static class ProgramEntry
{
    private static readonly CompilerCommandFactory Factory = new();

    public static int Run(string[] rawArgs)
    {
        try
        {
            var parsed = CliParser.Parse(rawArgs, Factory);

            if (parsed.ShowHelp)
            {
                return Factory.Create("help").Execute(Array.Empty<string>());
            }

            if (parsed.RunSample)
            {
                return RunSample();
            }

            if (parsed.Command is not null)
            {
                return Factory.Create(parsed.Command).Execute(parsed.CommandArgs);
            }

            if (parsed.SourcePath is not null)
            {
                return Factory.Create("interpret").Execute(new[] { parsed.SourcePath });
            }

            return RunSample();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine();
            Console.Error.WriteLine(CompilerCommandFactory.GetUsageText());
            return 1;
        }
    }

    private static int RunSample()
    {
        var defaultSamplePath = GetDefaultSamplePath();

        if (!File.Exists(defaultSamplePath))
        {
            Console.Error.WriteLine("The bundled sample.bas file was not found.");
            return 1;
        }

        return Factory.Create("interpret").Execute(new[] { defaultSamplePath });
    }

    private static string GetDefaultSamplePath()
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "sample.bas"));
    }
}

