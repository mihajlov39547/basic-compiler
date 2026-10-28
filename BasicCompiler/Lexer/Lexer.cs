namespace BasicCompiler.Lexer;

public sealed class Lexer
{
    private readonly string _source;
    private int _position;

    public Lexer(string source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public IReadOnlyList<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (!IsAtEnd())
        {
            var current = Peek();

            if (char.IsWhiteSpace(current))
            {
                Advance();
                continue;
            }

            if (char.IsLetter(current) || current == '_')
            {
                tokens.Add(ReadIdentifierOrKeyword());
                continue;
            }

            if (char.IsDigit(current))
            {
                tokens.Add(ReadNumber());
                continue;
            }

            if (current == '"')
            {
                tokens.Add(ReadString());
                continue;
            }

            tokens.Add(ReadPunctuation());
        }

        tokens.Add(new Token(TokenKind.EndOfFile, "", _position));
        return tokens;
    }

    private Token ReadIdentifierOrKeyword()
    {
        var start = _position;

        while (!IsAtEnd() && (char.IsLetterOrDigit(Peek()) || Peek() == '_'))
        {
            Advance();
        }

        var text = _source.Substring(start, _position - start);
        var kind = GetKeywordKind(text);
        return new Token(kind, text, start);
    }

    private Token ReadNumber()
    {
        var start = _position;

        while (!IsAtEnd() && char.IsDigit(Peek()))
        {
            Advance();
        }

        if (!IsAtEnd() && Peek() == '.')
        {
            Advance();

            while (!IsAtEnd() && char.IsDigit(Peek()))
            {
                Advance();
            }
        }

        var text = _source.Substring(start, _position - start);
        return new Token(TokenKind.Number, text, start);
    }

    private Token ReadString()
    {
        var start = _position;
        Advance(); // opening quote

        while (!IsAtEnd() && Peek() != '"')
        {
            if (Peek() == '\\')
            {
                Advance();
                if (!IsAtEnd())
                {
                    Advance();
                }
            }
            else
            {
                Advance();
            }
        }

        if (IsAtEnd())
        {
            throw new InvalidOperationException("Unterminated string literal.");
        }

        Advance(); // closing quote
        var text = _source.Substring(start, _position - start);
        return new Token(TokenKind.String, text, start);
    }

    private Token ReadPunctuation()
    {
        var start = _position;
        var current = Advance();

        return current switch
        {
            '+' => new Token(TokenKind.Plus, "+", start),
            '-' => new Token(TokenKind.Minus, "-", start),
            '*' => new Token(TokenKind.Star, "*", start),
            '/' => new Token(TokenKind.Slash, "/", start),
            ':' when Match('=') => new Token(TokenKind.Assign, ":=", start),
            ':' => new Token(TokenKind.Colon, ":", start),
            '=' when Match('=') => new Token(TokenKind.EqualsEquals, "==", start),
            '=' => new Token(TokenKind.Equals, "=", start),
            '!' when Match('=') => new Token(TokenKind.NotEquals, "!=", start),
            '!' => new Token(TokenKind.Not, "!", start),
            '&' when Match('&') => new Token(TokenKind.AndAnd, "&&", start),
            '|' when Match('|') => new Token(TokenKind.OrOr, "||", start),
            '<' when Match('=') => new Token(TokenKind.LessThanOrEqual, "<=", start),
            '<' => new Token(TokenKind.LessThan, "<", start),
            '>' when Match('=') => new Token(TokenKind.GreaterThanOrEqual, ">=", start),
            '>' => new Token(TokenKind.GreaterThan, ">", start),
            ';' => new Token(TokenKind.Semicolon, ";", start),
            '(' => new Token(TokenKind.LParen, "(", start),
            ')' => new Token(TokenKind.RParen, ")", start),
            '[' => new Token(TokenKind.LBracket, "[", start),
            ']' => new Token(TokenKind.RBracket, "]", start),
            ',' => new Token(TokenKind.Comma, ",", start),
            '.' => new Token(TokenKind.Dot, ".", start),
            _ => throw new InvalidOperationException($"Unexpected character '{current}'.")
        };
    }

    private bool Match(char expected)
    {
        if (IsAtEnd() || Peek() != expected)
        {
            return false;
        }

        Advance();
        return true;
    }

    private char Advance()
    {
        return _source[_position++];
    }

    private char Peek() => IsAtEnd() ? '\0' : _source[_position];
    private bool IsAtEnd() => _position >= _source.Length;

    private static TokenKind GetKeywordKind(string text)
    {
        return text switch
        {
            "let" => TokenKind.Let,
            "print" => TokenKind.Print,
            "if" => TokenKind.If,
            "else" => TokenKind.Else,
            "then" => TokenKind.Then,
            "endif" => TokenKind.EndIf,
            "var" => TokenKind.Var,
            "int" => TokenKind.Int,
            "bool" => TokenKind.Bool,
            "true" => TokenKind.True,
            "false" => TokenKind.False,
            "while" => TokenKind.While,
            "do" => TokenKind.Do,
            "endwhile" => TokenKind.EndWhile,
            _ => TokenKind.Identifier
        };
    }
}
