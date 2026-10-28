using BasicCompiler.Lexer;
using BasicCompiler.Runtime;
using BasicCompiler.Syntax;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: dotnet run --project BasicCompiler.csproj -- <path-to-sample.bas>");
    return 1;
}

var sourcePath = args[0];
if (!File.Exists(sourcePath))
{
    Console.Error.WriteLine($"File not found: {sourcePath}");
    return 1;
}

var source = File.ReadAllText(sourcePath);
var lexer = new Lexer(source);
var tokens = lexer.Tokenize();

Console.WriteLine($"MiniBASIC source: {sourcePath}");
Console.WriteLine("\nTokens:");
foreach (var token in tokens)
{
    if (token.Kind != TokenKind.EndOfFile)
    {
        Console.WriteLine($"{token.Kind}: {token.Text}");
    }
}

var parser = new Parser(tokens);
var statements = parser.ParseProgram();

Console.WriteLine("\nAST:");
foreach (var statement in statements)
{
    Console.WriteLine(statement);
}

var interpreter = new Interpreter();
var result = interpreter.Execute(statements);

Console.WriteLine($"\nExecution result: {result}");
return 0;

