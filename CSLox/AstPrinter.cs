using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public class AstPrinter : IVisitor<string>
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
        public string VisitBinary(Binary expr)
        {
            return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGrouping(Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteral(Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitUnary(Unary expr)
        {
            return Parenthesize(expr.Op.Lexeme, expr.Right);
        }
    }
}
