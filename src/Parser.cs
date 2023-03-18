using Lox.Ast;

namespace Lox;

public class Parser
{
    public List<Token> Tokens { get; }
    public int Current { get; private set; }

    public Parser(List<Token> tokens)
    {
        Tokens = tokens;
    }

    public Expr Expression() => Equality();
    public Expr Equality() => MakeBinaryMatch(Comparision, TokenType.BangEqual, TokenType.EqualEqual);
    public Expr Comparision() => MakeBinaryMatch(Term, TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual);
    public Expr Term() => MakeBinaryMatch(Factor, TokenType.Plus, TokenType.Minus);
    public Expr Factor() => MakeBinaryMatch(Unary, TokenType.Star, TokenType.Slash);

    public Expr Unary()
    {
        if (Match(TokenType.Bang, TokenType.Minus))
        {
            var op = Previous();
            var right = Unary();
            return new UnaryExpr(op, right);
        }

        return Primary();
    }

    public Expr Primary()
    {
        if (Match(TokenType.False)) return new LiteralExpr("false");
        if (Match(TokenType.True)) return new LiteralExpr("true");
        if (Match(TokenType.Nil)) return new LiteralExpr("nil");

        if (Match(TokenType.String, TokenType.Number))
            return new LiteralExpr(Previous().Literal);

        if (Match(TokenType.LeftParen))
        {
            var expr = Expression();
            Consume(TokenType.RightParen, "Expect ')' after expression.");
            return new GroupingExpr(expr);
        }

        throw Error(Peek(), "Expect expression.");
    }

    private Expr MakeBinaryMatch(Func<Expr> f, params TokenType[] tokens)
    {
        var expr = f();

        while (Match(tokens))
        {
            var op = Previous();
            var right = f();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private bool IsAtEnd() => Peek().Type == TokenType.EOF;
    private Token Advance() => Tokens[IsAtEnd() ? Current : ++Current];
    private Token Previous() => Tokens[Current - 1];
    public Token Peek() => Tokens[Current];
    public bool Check(TokenType type) => !IsAtEnd() && Peek().Type == type;
    public bool Check(params TokenType[] types) => !IsAtEnd() && types.Where(x => Peek().Type == x).Any();

    private bool Match(params TokenType[] types)
    {
        if (Check(types))
        {
            Advance();
            return true;
        }
        return false;
    }

    private Token Consume(TokenType tokenType, string errorMessage)
    {
        if (Check(tokenType))
            return Advance();

        throw Error(Peek(), errorMessage);
    }

    private ParseException Error(Token token, string message)
    {
        LoxInterpreter.Error(token, message);
        return new ParseException();
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.Semicolon)
                return;

            switch (Peek().Type)
            {
                case TokenType.Class:
                case TokenType.Fun:
                case TokenType.Var:
                case TokenType.For:
                case TokenType.If:
                case TokenType.While:
                case TokenType.Print:
                case TokenType.Return:
                    return;
            }

            Advance();
        }
    }
}

public class ParseException : Exception
{
}