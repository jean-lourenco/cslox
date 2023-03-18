using Lox.Ast;

namespace Lox;

public class ParseException : Exception { }

public class Parser
{
    public List<Token> Tokens { get; }
    public int Current { get; private set; }

    public Parser(List<Token> tokens)
    {
        Tokens = tokens;
    }

    public Expr? Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseException)
        {
            return null;
        }
    }

    private bool IsAtEnd() => Peek().Type == TokenType.EOF;
    private Token Advance() => Tokens[IsAtEnd() ? Current : ++Current];
    private Token Previous() => Tokens[Current - 1];
    private Token Peek() => Tokens[Current];
    private bool Check(TokenType type) => !IsAtEnd() && Peek().Type == type;
    private bool Check(params TokenType[] types) => !IsAtEnd() && types.Where(x => Peek().Type == x).Any();

    private Expr Expression() => Comma();
    private Expr Comma() => MakeBinaryMatch(Conditional, TokenType.Comma);
    private Expr Equality() => MakeBinaryMatch(Comparision, TokenType.BangEqual, TokenType.EqualEqual);
    private Expr Comparision() => MakeBinaryMatch(Term, TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual);
    private Expr Term() => MakeBinaryMatch(Factor, TokenType.Plus, TokenType.Minus);
    private Expr Factor() => MakeBinaryMatch(Unary, TokenType.Star, TokenType.Slash);

    private Expr Unary()
    {
        if (Match(TokenType.Bang, TokenType.Minus))
        {
            var op = Previous();
            var right = Unary();
            return new UnaryExpr(op, right);
        }

        return Primary();
    }

    private Expr Conditional()
    {
        var expr = Equality();

        if (Match(TokenType.Question))
        {
            var then = Expression();
            Consume(TokenType.Colon, "Expect ':' after then branch of conditional expression.");
            var @else = Conditional();
            expr = new ConditionalExpr(expr, then, @else);
        }

        return expr;
    }

    private Expr Primary()
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