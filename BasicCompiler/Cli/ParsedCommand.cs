namespace BasicCompiler.Cli;

internal sealed class ParsedCommand
{
    public ParsedCommand()
    {
        Command = null;
        CommandArgs = Array.Empty<string>();
        SourcePath = null;
        ShowHelp = false;
        RunSample = false;
    }

    public ParsedCommand(string? command, string[] commandArgs, string? sourcePath, bool showHelp, bool runSample)
    {
        Command = command;
        CommandArgs = commandArgs ?? Array.Empty<string>();
        SourcePath = sourcePath;
        ShowHelp = showHelp;
        RunSample = runSample;
    }

    public string? Command { get; }
    public string[] CommandArgs { get; }
    public string? SourcePath { get; }
    public bool ShowHelp { get; }
    public bool RunSample { get; }
}
