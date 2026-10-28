using BasicCompiler.Diagnostics;

namespace BasicCompiler.Lexer;

public sealed class Token
{
    public Token(TokenKind kind, string text, int position, string value = "-", SourceSpan? span = null)
    {
        Kind = kind;
        Text = text;
        Position = position;
        Value = value;
        Span = span ?? new SourceSpan(position, Math.Max(text.Length, 0), 0, 0);
    }

    public TokenKind Kind { get; }
    public string Text { get; }
    public int Position { get; }
    public string Value { get; }
    public SourceSpan Span { get; }
    public string Location => $"{Span.Line + 1}:{Span.Column + 1}";

    public override string ToString() => $"{Kind}: '{Text}' @ {Position}";
}
