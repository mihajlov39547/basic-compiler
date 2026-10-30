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

    public static string GetOutputDirectory()
    {
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var outputDirectory = Path.Combine(projectRoot, "output");
        return outputDirectory;
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
