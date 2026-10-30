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
    Not,
    And,
    Or,
    Negate,
    Jump,
    JumpIfFalse,
    Print
}
