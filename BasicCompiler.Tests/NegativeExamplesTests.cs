using BasicCompiler.Diagnostics;

namespace BasicCompiler.Tests;

public class NegativeExamplesTests
{
    [Theory]
    [InlineData("examples/invalid/01-syntax-error.bas")]
    [InlineData("examples/invalid/02-undefined-name.bas")]
    [InlineData("examples/invalid/03-type-mismatch.bas")]
    [InlineData("examples/invalid/04-operator-mismatch.bas")]
    public void InvalidExamples_ShouldProduceCompilerErrors(string relativePath)
    {
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var sourcePath = Path.Combine(projectRoot, relativePath);

        Assert.True(File.Exists(sourcePath), $"Expected invalid sample '{relativePath}' to exist.");

        var result = new MiniBasicCompiler().CompileFile(sourcePath);

        Assert.NotNull(result);
        Assert.True(result.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error),
            $"Expected '{relativePath}' to fail compilation with at least one error.");
    }
}
