using BasicCompiler.Bytecode;
using BasicCompiler.Diagnostics;
using BasicCompiler.Lexer;
using BasicCompiler.Syntax.AST;

namespace BasicCompiler.Output;

public static class ArtifactWriter
{
    public static string WriteLexerTokens(IReadOnlyList<Token> tokens, string sourcePath)
    {
        var outputDirectory = GetOutputDirectory();
        var filePath = Path.Combine(outputDirectory, "01-tokens.txt");

        var lines = new List<string>
        {
            $"Source: {Path.GetFileName(sourcePath)}",
            $"Generated: {DateTime.UtcNow:O}",
            $"Token count: {tokens.Count(t => t.Kind != TokenKind.EndOfFile)}",
            string.Empty,
            "#  Kind            Text                 Value               Span            Location",
            "----------------------------------------------------------------------------------------------"
        };

        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (token.Kind == TokenKind.EndOfFile)
            {
                lines.Add(FormatTokenRow(i, token));
                continue;
            }

            lines.Add(FormatTokenRow(i, token));
        }

        Directory.CreateDirectory(outputDirectory);
        File.WriteAllLines(filePath, lines);
        return filePath;
    }

    public static string WriteAst(IReadOnlyList<Statement> statements, string sourcePath)
    {
        var outputDirectory = GetOutputDirectory();
        var filePath = Path.Combine(outputDirectory, "02-ast.txt");

        var lines = new List<string>
        {
            $"Source: {Path.GetFileName(sourcePath)}",
            $"Generated: {DateTime.UtcNow:O}",
            $"Statement count: {statements.Count}",
            string.Empty,
            "AST",
            "----",
            AstPrinter.Print(statements)
        };

        Directory.CreateDirectory(outputDirectory);
        File.WriteAllLines(filePath, lines);
        return filePath;
    }

    public static string WriteSymbols(string symbolDump, string sourcePath)
    {
        var outputDirectory = GetOutputDirectory();
        var filePath = Path.Combine(outputDirectory, "03-symbols.txt");

        var lines = new List<string>
        {
            $"Source: {Path.GetFileName(sourcePath)}",
            $"Generated: {DateTime.UtcNow:O}",
            string.Empty,
            "Symbols",
            "--------",
            symbolDump
        };

        Directory.CreateDirectory(outputDirectory);
        File.WriteAllLines(filePath, lines);
        return filePath;
    }

    public static string WriteDiagnostics(IReadOnlyList<Diagnostic> diagnostics, string sourcePath)
    {
        var outputDirectory = GetOutputDirectory();
        var filePath = Path.Combine(outputDirectory, "04-diagnostics.txt");

        var lines = new List<string>
        {
            $"Source: {Path.GetFileName(sourcePath)}",
            $"Generated: {DateTime.UtcNow:O}",
            string.Empty,
            "Diagnostics",
            "-----------"
        };

        if (diagnostics.Count == 0)
        {
            lines.Add("No diagnostics.");
        }
        else
        {
            lines.AddRange(diagnostics.Select(d => d.ToString()));
        }

        Directory.CreateDirectory(outputDirectory);
        File.WriteAllLines(filePath, lines);
        return filePath;
    }

    public static string WriteRuntimeResult(object? runtimeResult, string sourcePath)
    {
        var outputDirectory = GetOutputDirectory();
        var filePath = Path.Combine(outputDirectory, "05-runtime.txt");

        var formatted = runtimeResult switch
        {
            null => "<null>",
            bool value => value.ToString(),
            string value => value,
            IFormattable formattable => formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
            _ => runtimeResult.ToString() ?? "<empty>"
        };

        var lines = new List<string>
        {
            $"Source: {Path.GetFileName(sourcePath)}",
            $"Generated: {DateTime.UtcNow:O}",
            string.Empty,
            "Execution result",
            "----------------",
            $"{formatted}"
        };

        Directory.CreateDirectory(outputDirectory);
        File.WriteAllLines(filePath, lines);
        return filePath;
    }

    public static string WritePythonCode(string pythonCode, string sourcePath)
    {
        var outputDirectory = GetOutputDirectory();
        var filePath = Path.Combine(outputDirectory, "08-python.py");

        Directory.CreateDirectory(outputDirectory);
        File.WriteAllText(filePath, pythonCode);
        return filePath;
    }

    public static string WriteBytecode(CompiledProgram program, string sourcePath)
    {
        var outputDirectory = GetOutputDirectory();
        var filePath = Path.Combine(outputDirectory, "06-bytecode.txt");

        var lines = new List<string>
        {
            $"Source: {Path.GetFileName(sourcePath)}",
            $"Generated: {DateTime.UtcNow:O}",
            $"Instruction count: {program.Instructions.Count}",
            $"Constant count: {program.Constants.Count}",
            string.Empty,
            "Bytecode program",
            "----------------",
            "PC | Opcode             | Operand | Meaning",
            "-------------------------------------------"
        };

        for (var i = 0; i < program.Instructions.Count; i++)
        {
            var instruction = program.Instructions[i];
            lines.Add($"{i,2} | {instruction.OpCode,-18} | {FormatOperand(instruction.Operand),-8} | {DescribeBytecodeInstruction(instruction, program.Constants)}");
        }

        lines.Add(string.Empty);
        lines.Add("Constants:");
        for (var i = 0; i < program.Constants.Count; i++)
        {
            var constant = program.Constants[i];
            lines.Add($"  [{i}] = {FormatConstant(constant)}");
        }

        Directory.CreateDirectory(outputDirectory);
        File.WriteAllLines(filePath, lines);
        return filePath;
    }

    public static string WriteVmTrace(IReadOnlyList<string> trace, string sourcePath)
    {
        var outputDirectory = GetOutputDirectory();
        var filePath = Path.Combine(outputDirectory, "07-vm-trace.txt");

        var lines = new List<string>
        {
            $"Source: {Path.GetFileName(sourcePath)}",
            $"Generated: {DateTime.UtcNow:O}",
            string.Empty,
            "Virtual Machine Trace",
            "--------------------",
            "Step | Opcode             | Action                               | Stack | Environment"
        };

        lines.AddRange(trace);

        Directory.CreateDirectory(outputDirectory);
        File.WriteAllLines(filePath, lines);
        return filePath;
    }

    public static string GetOutputDirectory()
    {
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var outputDirectory = Path.Combine(projectRoot, "output");
        return outputDirectory;
    }

    private static string DescribeBytecodeInstruction(Instruction instruction, IReadOnlyList<object?> constants)
    {
        return instruction.OpCode switch
        {
            OpCode.LoadConstant => $"push const[{instruction.Operand}] = {FormatConstant(constants[(int)(instruction.Operand ?? 0)])}",
            OpCode.LoadVariable => $"push env[\"{instruction.Operand}\"]",
            OpCode.StoreVariable => $"store top-of-stack into {instruction.Operand}",
            OpCode.Add => "pop right, left; push left + right",
            OpCode.Subtract => "pop right, left; push left - right",
            OpCode.Multiply => "pop right, left; push left * right",
            OpCode.Divide => "pop right, left; push left / right",
            OpCode.Equals => "pop right, left; push left == right",
            OpCode.NotEquals => "pop right, left; push left != right",
            OpCode.LessThan => "pop right, left; push left < right",
            OpCode.GreaterThan => "pop right, left; push left > right",
            OpCode.LessThanOrEqual => "pop right, left; push left <= right",
            OpCode.GreaterThanOrEqual => "pop right, left; push left >= right",
            OpCode.Not => "logical not",
            OpCode.And => "logical and",
            OpCode.Or => "logical or",
            OpCode.Negate => "negate top-of-stack",
            OpCode.Jump => $"jump to {instruction.Operand}",
            OpCode.JumpIfFalse => $"jump to {instruction.Operand} if condition is false",
            OpCode.Print => "print top-of-stack",
            OpCode.Halt => "stop execution",
            _ => "-"
        };
    }

    private static string FormatOperand(object? operand)
    {
        if (operand is null)
        {
            return "-";
        }

        return operand.ToString() ?? "-";
    }

    private static string FormatConstant(object? constant)
    {
        if (constant is null)
        {
            return "<null>";
        }

        if (constant is string text)
        {
            return $"\"{EscapeDisplay(text)}\"";
        }

        return constant.ToString() ?? "<empty>";
    }

    private static string FormatTokenRow(int index, Token token)
    {
        var kind = FormatKind(token.Kind);
        var text = FormatText(token.Text);
        var value = FormatValue(token.Value);
        var span = $"[{token.Span.Start}..{token.Span.Start + token.Span.Length})";
        var location = token.Location;

        return $"{index,-2} {kind,-14} {text,-22} {value,-18} {span,-16} {location}";
    }

    private static string FormatKind(TokenKind kind)
    {
        return kind switch
        {
            TokenKind.EndOfFile => "EOF",
            _ => kind.ToString()
        };
    }

    private static string FormatText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "\"\"";
        }

        return $"\"{EscapeDisplay(text)}\"";
    }

    private static string FormatValue(string value)
    {
        if (string.IsNullOrEmpty(value) || value == "-")
        {
            return "-";
        }

        return EscapeDisplay(value);
    }

    private static string EscapeDisplay(string input)
    {
        var escaped = input.Replace("\\", "\\\\");
        escaped = escaped.Replace("\n", "\\n");
        escaped = escaped.Replace("\r", "\\r");
        escaped = escaped.Replace("\t", "\\t");
        escaped = escaped.Replace("\"", "\\\"");
        return escaped;
    }
}
