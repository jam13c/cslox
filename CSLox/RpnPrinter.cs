using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public class RpnPrinter : IExprVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitAnonymousFunctionExpr(Expr.AnonymousFunction expr)
        {
            throw new NotImplementedException();
        }

        public string VisitAssignExpr(Expr.Assign expr)
        {
            throw new NotImplementedException();
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return $"{expr.Left.Accept(this)} {expr.Right.Accept(this)} {expr.Op.Lexeme}";           

        }

        public string VisitCallExpr(Expr.Call expr)
        {
            throw new NotImplementedException();
        }

        public string VisitExpressionStmt(Stmt.Expression stmt)
        {
            throw new NotImplementedException();
        }

        public string VisitGetExpr(Expr.Get expr)
        {
            throw new NotImplementedException();
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return expr.Expression.Accept(this);
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            if (expr.Value == null) return "nil" ;
            return expr.Value.ToString();
        }

        public string VisitLogicalExpr(Expr.Logical expr)
        {
            throw new NotImplementedException();
        }

        public string VisitPrintStmt(Stmt.Print stmt)
        {
            throw new NotImplementedException();
        }

        public string VisitSetExpr(Expr.Set expr)
        {
            throw new NotImplementedException();
        }

        public string VisitSuperExpr(Expr.Super expr)
        {
            throw new NotImplementedException();
        }

        public string VisitThisExpr(Expr.This expr)
        {
            throw new NotImplementedException();
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            throw new NotImplementedException();
        }

        public string VisitVariableExpr(Expr.Variable expr)
        {
            throw new NotImplementedException();
        }
    }
}
