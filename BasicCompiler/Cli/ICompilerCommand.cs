namespace BasicCompiler.Cli;

public interface ICompilerCommand
{
    int Execute(string[] args);
}
