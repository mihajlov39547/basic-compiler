using BasicCompiler.Syntax.AST;

namespace BasicCompiler.Bytecode;

public sealed class BytecodeCompiler
{
    public CompiledProgram Compile(IEnumerable<Statement> statements)
    {
        var instructions = new List<Instruction>();
        var constants = new List<object?>();

        foreach (var statement in statements)
        {
            CompileStatement(statement, instructions, constants);
        }

        instructions.Add(new Instruction(OpCode.Halt));
        return new CompiledProgram(instructions, constants);
    }

    private static void CompileStatement(Statement statement, ICollection<Instruction> instructions, ICollection<object?> constants)
    {
        switch (statement)
        {
            case VariableDeclaration declaration:
                if (declaration.Initializer is not null)
                {
                    CompileExpression(declaration.Initializer, instructions, constants);
                    instructions.Add(new Instruction(OpCode.StoreVariable, declaration.Name));
                }
                break;

            case PrintStatement printStatement:
                CompileExpression(printStatement.Expression, instructions, constants);
                instructions.Add(new Instruction(OpCode.Print));
                break;

            case IfStatement ifStatement:
                CompileExpression(ifStatement.Condition, instructions, constants);
                instructions.Add(new Instruction(OpCode.JumpIfFalse, 0));
                CompileStatement(ifStatement.ThenStatement, instructions, constants);
                if (ifStatement.ElseStatement is not null)
                {
                    CompileStatement(ifStatement.ElseStatement, instructions, constants);
                }
                break;
        }
    }

    private static void CompileExpression(Expression expression, ICollection<Instruction> instructions, ICollection<object?> constants)
    {
        switch (expression)
        {
            case LiteralExpression literal:
                constants.Add(literal.Value);
                instructions.Add(new Instruction(OpCode.LoadConstant, constants.Count - 1));
                break;

            case VariableExpression variable:
                instructions.Add(new Instruction(OpCode.LoadVariable, variable.Name));
                break;

            case BinaryExpression binary:
                CompileExpression(binary.Left, instructions, constants);
                CompileExpression(binary.Right, instructions, constants);
                instructions.Add(binary.Operator switch
                {
                    "+" => new Instruction(OpCode.Add),
                    "-" => new Instruction(OpCode.Subtract),
                    "*" => new Instruction(OpCode.Multiply),
                    "/" => new Instruction(OpCode.Divide),
                    "==" => new Instruction(OpCode.Equals),
                    "!=" => new Instruction(OpCode.NotEquals),
                    "<" => new Instruction(OpCode.LessThan),
                    ">" => new Instruction(OpCode.GreaterThan),
                    "<=" => new Instruction(OpCode.LessThanOrEqual),
                    ">=" => new Instruction(OpCode.GreaterThanOrEqual),
                    _ => throw new InvalidOperationException($"Unsupported bytecode operator '{binary.Operator}'.")
                });
                break;
        }
    }
}

public sealed record CompiledProgram(IReadOnlyList<Instruction> Instructions, IReadOnlyList<object?> Constants);
