using BasicCompiler.Bytecode;
using BasicCompiler.Lexer;
using BasicCompiler.Output;
using BasicCompiler.Syntax;
using LexerClass = BasicCompiler.Lexer.Lexer;

namespace BasicCompiler.Cli;

public sealed class BytecodeCommand : CompilerCommandBase
{
    public override int Execute(string[] args)
    {
        var sourcePath = RequireSourcePath(args);
        var source = ReadSource(sourcePath);
        var tokens = new LexerClass(source).Tokenize();
        var statements = new Parser(tokens).ParseProgram();

        var compiler = new BytecodeCompiler();
        var program = compiler.Compile(statements);
        var trace = new VirtualMachine().RunWithTrace(program);

        var bytecodePath = ArtifactWriter.WriteBytecode(program, sourcePath);
        var tracePath = ArtifactWriter.WriteVmTrace(trace.Trace, sourcePath);

        Console.WriteLine($"MiniBASIC source: {sourcePath}");
        Console.WriteLine($"Bytecode artifact saved to: {bytecodePath}");
        Console.WriteLine($"VM trace saved to: {tracePath}");
        Console.WriteLine($"Final VM result: {trace.FinalValue}");

        foreach (var step in trace.Trace)
        {
            Console.WriteLine(step);
        }

        return 0;
    }
}
