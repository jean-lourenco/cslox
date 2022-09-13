namespace Lox.Ast;

public class AstPrinter : IVisitor<string>
{
    public string Print(Expr expr) => expr.Accept(this);

    public string VisitBinaryExpr(BinaryExpr expr) =>
        $"({expr.Op.Lexeme} {expr.Left.Accept(this)} {expr.Right.Accept(this)})";

    public string VisitGroupingExpr(GroupingExpr expr) =>
        $"(group {expr.Expr.Accept(this)})";

    public string VisitLiteralExpr(LiteralExpr expr) =>
        expr.Value?.ToString() ?? "nil";

    public string VisitUnaryExpr(UnaryExpr expr) =>
        $"({expr.Op.Lexeme} {expr.Right.Accept(this)})";
}