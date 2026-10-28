namespace BasicCompiler.Bytecode;

public sealed record Instruction(OpCode OpCode, object? Operand = null)
{
    public override string ToString() => Operand is null ? $"{OpCode}" : $"{OpCode} {Operand}";
}
