namespace Lox.Ast;

public interface IVisitor<T>
{
    T VisitBinaryExpr(BinaryExpr expr);
    T VisitGroupingExpr(GroupingExpr expr);
    T VisitLiteralExpr(LiteralExpr expr);
    T VisitUnaryExpr(UnaryExpr expr);
}

public abstract record Expr()
{
    public abstract T Accept<T>(IVisitor<T> visitor);
}

public record BinaryExpr(Expr Left, Token Op, Expr Right) : Expr
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitBinaryExpr(this);
}

public record GroupingExpr(Expr Expr) : Expr
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitGroupingExpr(this);
}

public record LiteralExpr(object? Value) : Expr
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitLiteralExpr(this);
}

public record UnaryExpr(Token Op, Expr Right) : Expr
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitUnaryExpr(this);
}