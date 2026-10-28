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

    [Fact]
    public void HelpText_ListsCommandsAndExamples()
    {
        var helpText = CompilerCommandFactory.GetUsageText();

        Assert.Contains("Examples:", helpText, StringComparison.Ordinal);
        Assert.Contains("lex", helpText, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("parse", helpText, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("check", helpText, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("interpret", helpText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Diagnostic_ToString_IsHumanReadable()
    {
        var diagnostic = new BasicCompiler.Diagnostics.Diagnostic(
            "If condition must evaluate to bool.",
            BasicCompiler.Diagnostics.DiagnosticSeverity.Error,
            new BasicCompiler.Diagnostics.SourceSpan(10, 12, 3, 4));

        var text = diagnostic.ToString();

        Assert.Contains("Error:", text, StringComparison.Ordinal);
        Assert.Contains("If condition must evaluate to bool.", text, StringComparison.Ordinal);
        Assert.Contains("line 3", text, StringComparison.OrdinalIgnoreCase);
    }
}
