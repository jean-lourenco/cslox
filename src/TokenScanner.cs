using System.Collections;
using System.Text.RegularExpressions;

namespace Lox;

public enum TokenType
{
    // Single character tokens
    LeftParen, RightParen, LeftBrace, RightBrace,
    Comma, Dot, Minus, Plus, Semicolon, Slash, Star,

    // One or two tokens
    Bang, BangEqual, Equal, EqualEqual,
    Greater, GreaterEqual, Less, LessEqual,

    // Literals
    Identifier, String, Number,

    // Keywords
    And, Class, Else, False, Fun, For, If, Nil, Or,
    Print, Return, Super, This, True, Var, While,

    EOF,
}

public class Token
{
    public TokenType Type { get; }
    public string Lexeme { get; }
    public object? Literal { get; }
    public int Line { get; }

    public Token(TokenType type, string lexeme, object? literal, int line)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
        Line = line;
    }

    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal}";
    }
}

public class Scanner
{
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;
    private readonly string _source;
    private readonly IList<Token> _tokens = new List<Token>();
    private bool _isEnd => _current >= _source.Length;
    private static readonly IDictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>
    {
        { "and", TokenType.And },
        { "class", TokenType.Class },
        { "else", TokenType.Else },
        { "false", TokenType.False },
        { "for", TokenType.For },
        { "fun", TokenType.Fun },
        { "if", TokenType.If },
        { "nil", TokenType.Nil },
        { "or", TokenType.Or },
        { "print", TokenType.Print },
        { "return", TokenType.Return },
        { "super", TokenType.Super },
        { "this", TokenType.This },
        { "true", TokenType.True },
        { "var", TokenType.Var },
        { "while", TokenType.While },
    };

    public Scanner(string source)
    {
        _source = source;
    }

    public IEnumerable<Token> ScanTokens()
    {
        while (!_isEnd)
        {
            _start = _current;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.EOF, "", null, _line));
        return _tokens;
    }

    private void ScanToken()
    {
        var c = Advance();
        switch (c)
        {
            case '(': AddToken(TokenType.LeftParen); break;
            case ')': AddToken(TokenType.RightParen); break;
            case '{': AddToken(TokenType.LeftBrace); break;
            case '}': AddToken(TokenType.RightBrace); break;
            case ',': AddToken(TokenType.Comma); break;
            case '.': AddToken(TokenType.Dot); break;
            case '-': AddToken(TokenType.Minus); break;
            case '+': AddToken(TokenType.Plus); break;
            case ';': AddToken(TokenType.Semicolon); break;
            case '*': AddToken(TokenType.Star); break;
            case '!': AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang); break;
            case '=': AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal); break;
            case '>': AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater); break;
            case '<': AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less); break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !_isEnd)
                        Advance();
                }
                else
                    AddToken(TokenType.Slash);
                break;
            case '\t':
            case '\r':
            case ' ':
                break;
            case '\n': _line++; break;
            case '"': ProcessString(); break;
            default:
                if (IsDigit(c))
                    ProcessNumber();
                else if (IsAlpha(c))
                    ProcessIdentifier();
                else
                    LoxEntryPoint.Error(_line, $"Unexpected character {c}");

                break;
        };
    }

    private char Advance() => _source[_current++];
    private void AddToken(TokenType type) => AddToken(type, null);
    private void AddToken(TokenType type, object? literal) =>
        _tokens.Add(new Token(type, _source.Substring(_start, _current - _start), literal, _line));
    private bool Match(char c)
    {
        if (_isEnd || _source[_current] != c)
            return false;

        _current++;
        return true;
    }

    private char Peek()
    {
        if (_isEnd) return '\0';
        return _source[_current];
    }

    private bool IsDigit(char c) => c >= '0' && c <= '9';
    private bool IsAlpha(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    private bool IsAlphaNumeric(char c) => IsDigit(c) || IsAlpha(c);

    private char PeekNext()
    {
        if (_current + 1 >= _source.Length) return '\0';
        return _source[_current + 1];
    }

    private void ProcessString()
    {
        while (Peek() != '"' && !_isEnd)
        {
            // TODO: If the lexeme was passed by arguments instead of recalculated on the AddToken method
            // I could potencially alter the lexeme here to change the \n to a space char, for example
            if (Peek() == '\n')
                _line++;
            Advance();
        }

        if (_isEnd)
        {
            LoxEntryPoint.Error(_line, "Unterminated String");
            return;
        }

        Advance();
        System.Console.WriteLine(_start);
        System.Console.WriteLine(_current);
        var literal = _source.Substring(_start + 1, _current - _start - 2);
        AddToken(TokenType.String, literal);
    }

    private void ProcessNumber()
    {
        while (IsDigit(Peek()))
            Advance();

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();
            while (IsDigit(Peek()))
                Advance();
        }

        var raw = _source.Substring(_start, _current - _start);
        var success = double.TryParse(raw, out var literal);

        if (!success)
        {
            LoxEntryPoint.Error(_line, $"Could not convert to number the expression {raw}");
            return;
        }

        AddToken(TokenType.Number, literal);
    }

    private void ProcessIdentifier()
    {
        while (IsAlphaNumeric(Peek()))
            Advance();

        var raw = _source.Substring(_start, _current - _start);
        var type = _keywords.ContainsKey(raw) ? _keywords[raw] : TokenType.Identifier;
        AddToken(type);
    }
}