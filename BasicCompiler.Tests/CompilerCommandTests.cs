using BasicCompiler.Cli;

namespace BasicCompiler.Tests;

public class CompilerCommandTests
{
    [Fact]
    public void CommandFactory_RecognizesBuiltInCommands()
    {
        var factory = new CompilerCommandFactory();

        Assert.Contains("lex", factory.GetAvailableCommands());
        Assert.Contains("parse", factory.GetAvailableCommands());
        Assert.Contains("check", factory.GetAvailableCommands());
        Assert.Contains("interpret", factory.GetAvailableCommands());
    }

    [Fact]
    public void CommandFactory_CreatesInterpretCommand()
    {
        var command = new CompilerCommandFactory().Create("interpret");

        Assert.IsType<InterpretCommand>(command);
    }
}
