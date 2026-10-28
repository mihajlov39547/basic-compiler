namespace BasicCompiler.Lexer;

public enum TokenKind
{
    EndOfFile,
    Identifier,
    Number,
    String,
    Keyword,

    // Punctuation
    Plus,
    Minus,
    Star,
    Slash,
    Equals,
    Assign,
    Colon,
    Semicolon,
    LParen,
    RParen,
    LBracket,
    RBracket,
    Comma,
    Dot,

    // Comparison
    EqualsEquals,
    NotEquals,
    LessThan,
    GreaterThan,
    LessThanOrEqual,
    GreaterThanOrEqual,
    AndAnd,
    OrOr,
    Not,

    // Keywords
    Let,
    Print,
    If,
    Else,
    Then,
    EndIf,
    Var,
    Int,
    Bool,
    True,
    False,
    While,
    EndWhile,

    // Special
    Unknown
}
