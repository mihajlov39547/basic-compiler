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

    private static void CompileStatement(Statement statement, List<Instruction> instructions, List<object?> constants)
    {
        switch (statement)
        {
            case BlockStatement block:
                foreach (var item in block.Statements)
                {
                    CompileStatement(item, instructions, constants);
                }
                break;

            case VariableDeclaration declaration:
                if (declaration.Initializer is not null)
                {
                    CompileExpression(declaration.Initializer, instructions, constants);
                    instructions.Add(new Instruction(OpCode.StoreVariable, declaration.Name));
                }
                break;

            case AssignmentStatement assignment:
                CompileExpression(assignment.Expression, instructions, constants);
                instructions.Add(new Instruction(OpCode.StoreVariable, assignment.Name));
                break;

            case PrintStatement printStatement:
                CompileExpression(printStatement.Expression, instructions, constants);
                instructions.Add(new Instruction(OpCode.Print));
                break;

            case IfStatement ifStatement:
                CompileExpression(ifStatement.Condition, instructions, constants);

                var falseJumpIndex = instructions.Count;
                instructions.Add(new Instruction(OpCode.JumpIfFalse, 0));

                CompileStatement(ifStatement.ThenStatement, instructions, constants);

                if (ifStatement.ElseStatement is not null)
                {
                    var endJumpIndex = instructions.Count;
                    instructions.Add(new Instruction(OpCode.Jump, 0));
                    var elseStartIndex = instructions.Count;
                    instructions[falseJumpIndex] = instructions[falseJumpIndex] with { Operand = elseStartIndex };
                    CompileStatement(ifStatement.ElseStatement, instructions, constants);
                    instructions[endJumpIndex] = instructions[endJumpIndex] with { Operand = instructions.Count };
                }
                else
                {
                    instructions[falseJumpIndex] = instructions[falseJumpIndex] with { Operand = instructions.Count };
                }
                break;

            case WhileStatement whileStatement:
                var loopStartIndex = instructions.Count;
                CompileExpression(whileStatement.Condition, instructions, constants);

                var loopExitIndex = instructions.Count;
                instructions.Add(new Instruction(OpCode.JumpIfFalse, 0));

                CompileStatement(whileStatement.Body, instructions, constants);
                instructions.Add(new Instruction(OpCode.Jump, loopStartIndex));
                instructions[loopExitIndex] = instructions[loopExitIndex] with { Operand = instructions.Count };
                break;

            default:
                throw new InvalidOperationException($"Statement type '{statement.GetType().Name}' is not supported by bytecode lowering.");
        }
    }

    private static void CompileExpression(Expression expression, List<Instruction> instructions, List<object?> constants)
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

            case UnaryExpression unary:
                CompileExpression(unary.Operand, instructions, constants);
                instructions.Add(unary.Operator switch
                {
                    "!" => new Instruction(OpCode.Not),
                    "-" => new Instruction(OpCode.Negate),
                    _ => throw new InvalidOperationException($"Unsupported bytecode unary operator '{unary.Operator}'.")
                });
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
                    "&&" => new Instruction(OpCode.And),
                    "||" => new Instruction(OpCode.Or),
                    _ => throw new InvalidOperationException($"Unsupported bytecode operator '{binary.Operator}'.")
                });
                break;

            default:
                throw new InvalidOperationException($"Expression type '{expression.GetType().Name}' is not supported by bytecode lowering.");
        }
    }
}

public sealed record CompiledProgram(IReadOnlyList<Instruction> Instructions, IReadOnlyList<object?> Constants);
