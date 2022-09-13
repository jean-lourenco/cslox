namespace Lox.Ast;

public interface IVisitor<T>
{
    T visitBinaryExpr(Binary expr);
    T visitGroupingExpr(Grouping expr);
    T visitLiteralExpr(Literal expr);
    T visitUnaryExpr(Unary expr);
}

public abstract record Expr()
{
    public abstract T Accept<T>(IVisitor<T> visitor);
}

public record Binary(Expr left, Token op, Expr right) : Expr
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.visitBinaryExpr(this);
}

public record Grouping(Expr expr) : Expr
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.visitGroupingExpr(this);
}

public record Literal(object value) : Expr
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.visitLiteralExpr(this);
}

public record Unary(Token op, Expr right) : Expr
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.visitUnaryExpr(this);
}