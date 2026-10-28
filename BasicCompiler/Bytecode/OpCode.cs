namespace BasicCompiler.Bytecode;

public enum OpCode
{
    Halt,
    LoadConstant,
    LoadVariable,
    StoreVariable,
    Add,
    Subtract,
    Multiply,
    Divide,
    Equals,
    NotEquals,
    LessThan,
    GreaterThan,
    LessThanOrEqual,
    GreaterThanOrEqual,
    JumpIfFalse,
    Print
}
