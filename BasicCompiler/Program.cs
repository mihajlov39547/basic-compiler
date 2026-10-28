using BasicCompiler.Bytecode;
using BasicCompiler.Lexer;
using BasicCompiler.Runtime;
using BasicCompiler.Semantics;
using BasicCompiler.Syntax;

var source = """
let x = 10;
let y = x + 5;
print y;
""";

Console.WriteLine("MiniBasic compiler demo");
Console.WriteLine("Source:");
Console.WriteLine(source);

var lexer = new Lexer(source);
var tokens = lexer.Tokenize();

Console.WriteLine("\nTokens:");
foreach (var token in tokens)
{
    if (token.Kind != TokenKind.EndOfFile)
    {
        Console.WriteLine(token);
    }
}

var parser = new Parser(tokens);
var statements = parser.ParseProgram();

Console.WriteLine("\nAST:");
foreach (var statement in statements)
{
    Console.WriteLine(statement);
}

var resolver = new NameResolver();
resolver.Resolve(statements);

var checker = new TypeChecker();
var diagnostics = checker.Check(statements);
if (diagnostics.Count > 0)
{
    Console.WriteLine("\nDiagnostics:");
    foreach (var diagnostic in diagnostics)
    {
        Console.WriteLine(diagnostic);
    }
}

var interpreter = new Interpreter();
var result = interpreter.Execute(statements);
Console.WriteLine($"\nRuntime result: {result}");

var bytecodeCompiler = new BytecodeCompiler();
var compiledProgram = bytecodeCompiler.Compile(statements);
Console.WriteLine("\nBytecode:");
foreach (var instruction in compiledProgram.Instructions)
{
    Console.WriteLine(instruction);
}

