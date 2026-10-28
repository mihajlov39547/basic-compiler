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

    [Fact]
    public void MiniBasicCompilerFacade_CompilesSourceIntoPipelineResult()
    {
        var compiler = new BasicCompiler.MiniBasicCompiler();
        var result = compiler.Compile("var total := 10\nprint(total)");

        Assert.NotNull(result);
        Assert.NotNull(result.Tokens);
        Assert.NotEmpty(result.Tokens);
        Assert.NotEmpty(result.Statements);
        Assert.Equal(0, result.Diagnostics.Count);
        Assert.Equal("total", ((BasicCompiler.Syntax.AST.VariableDeclaration)result.Statements[0]).Name);
    }

    [Fact]
    public void InterpretCommand_WritesFullArtifactsForTheSourceProgram()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"mini-basic-phase10-{Guid.NewGuid():N}.bas");
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var samplePath = Path.Combine(projectRoot, "sample.bas");
        var artifactDirectory = Path.Combine(projectRoot, "output");

        File.WriteAllText(tempPath, File.ReadAllText(samplePath));

        try
        {
            var result = new InterpretCommand().Execute(new[] { tempPath });

            Assert.Equal(0, result);
            Assert.True(File.Exists(Path.Combine(artifactDirectory, "01-tokens.txt")));
            Assert.True(File.Exists(Path.Combine(artifactDirectory, "02-ast.txt")));

            var tokensText = File.ReadAllText(Path.Combine(artifactDirectory, "01-tokens.txt"));
            Assert.Contains("Source:", tokensText, StringComparison.Ordinal);
            Assert.Contains("EOF", tokensText, StringComparison.Ordinal);
            Assert.Contains("compiler start", tokensText, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(tempPath);
        }
    }
}
