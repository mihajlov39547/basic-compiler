using BasicCompiler.Bytecode;
using BasicCompiler.Cli;
using BasicCompiler.Lexer;
using BasicCompiler.Syntax;
using LexerClass = BasicCompiler.Lexer.Lexer;

namespace BasicCompiler.Tests;

public class BytecodeTests
{
    [Fact]
    public void BytecodeCompiler_CompilesAndRunsLoopProgram()
    {
        var source = "var total := 2\nvar counter := 0\nwhile counter < 3 do\n    total := total + 1\n    counter := counter + 1\nendwhile\nprint(total)";

        var tokens = new LexerClass(source).Tokenize();
        var statements = new Parser(tokens).ParseProgram();

        var program = new BytecodeCompiler().Compile(statements);
        var result = new VirtualMachine().Run(program);

        Assert.Equal(5d, Convert.ToDouble(result), 0.0001);
        Assert.NotEmpty(program.Instructions);
    }

    [Fact]
    public void BytecodeCommand_WritesBytecodeAndVmTraceArtifacts()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"mini-basic-bytecode-{Guid.NewGuid():N}.bas");
        var source = "var value := 2\nprint(value + 3)";

        File.WriteAllText(tempPath, source);

        try
        {
            var exitCode = new BytecodeCommand().Execute(new[] { tempPath });

            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..")), "output", "06-bytecode.txt")));
            Assert.True(File.Exists(Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..")), "output", "07-vm-trace.txt")));
        }
        finally
        {
            File.Delete(tempPath);
        }
    }
}
