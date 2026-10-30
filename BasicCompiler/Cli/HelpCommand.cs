namespace BasicCompiler.Cli;

public sealed class HelpCommand : CompilerCommandBase
{
    public override int Execute(string[] args)
    {
        Console.WriteLine(CompilerCommandFactory.GetUsageText());
        return 0;
    }
}
