using BasicCompiler.Lexer;
using BasicCompiler.Output;
using BasicCompiler.Runtime;
using BasicCompiler.Syntax;

// Activity 10: Turn the compiler flow into a visible CLI pipeline for students.
if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: dotnet run --project BasicCompiler.csproj -- <path-to-sample.bas>");
    return 1;
}

// Activity 1: Load the source program from disk.
var sourcePath = args[0];
if (!File.Exists(sourcePath))
{
    Console.Error.WriteLine($"File not found: {sourcePath}");
    return 1;
}

// Activity 2: Read the raw source text into memory.
var source = File.ReadAllText(sourcePath);

// Activity 5: Lex the source into tokens and save the result for later study.
var lexer = new Lexer(source);
var tokens = lexer.Tokenize();
var lexerArtifactPath = ArtifactWriter.WriteLexerTokens(tokens, sourcePath);

Console.WriteLine($"MiniBASIC source: {sourcePath}");
Console.WriteLine($"\nLexer artifact saved to: {lexerArtifactPath}");
Console.WriteLine("\nTokens:");
foreach (var token in tokens)
{
    if (token.Kind != TokenKind.EndOfFile)
    {
        Console.WriteLine($"{token.Kind}: {token.Text}");
    }
}

// Activity 6: Parse the token stream into AST statements and save the result for later study.
var parser = new Parser(tokens);
var statements = parser.ParseProgram();
var astArtifactPath = ArtifactWriter.WriteAst(statements, sourcePath);

Console.WriteLine($"\nAST artifact saved to: {astArtifactPath}");
Console.WriteLine("\nAST:");
foreach (var statement in statements)
{
    Console.WriteLine(statement);
}

// Activity 9: Execute the resulting program through the reference interpreter.
var interpreter = new Interpreter();
var result = interpreter.Execute(statements);

Console.WriteLine($"\nExecution result: {result}");
return 0;

