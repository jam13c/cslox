using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public class AstPrinter : IExprVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }
        private string Parenthesize(string name, params Expr[] exprs)
        {
            var sb = new StringBuilder();
            sb.Append("(").Append(name);
            foreach(var expr in exprs)
            {
                sb.Append(" ").Append(expr.Accept(this));
            }
            sb.Append(")");
            return sb.ToString();
        }
        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.Op.Lexeme, expr.Right);
        }

        public string VisitExpressionStmt(Stmt.Expression stmt)
        {
            throw new NotImplementedException();
        }

        public string VisitPrintStmt(Stmt.Print stmt)
        {
            throw new NotImplementedException();
        }

        public string VisitVariableExpr(Expr.Variable expr)
        {
            throw new NotImplementedException();
        }

        public string VisitAssignExpr(Expr.Assign expr)
        {
            throw new NotImplementedException();
        }

        public string VisitLogicalExpr(Expr.Logical expr)
        {
            throw new NotImplementedException();
        }

        public string VisitCallExpr(Expr.Call expr)
        {
            throw new NotImplementedException();
        }
    }
}
