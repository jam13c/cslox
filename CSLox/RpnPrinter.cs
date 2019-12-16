using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public class RpnCalculator : IVisitor<double>
    {
        public double Evaluate(Expr expr) => expr.Accept(this);
        public double VisitBinary(Binary expr)
        {
            var left = expr.Left.Accept(this);
            var right = expr.Right.Accept(this);
            switch (expr.Op.Type)
            {
                case TokenType.Star: return left * right;
                case TokenType.Slash: return left / right;
                case TokenType.Plus: return left + right;
                case TokenType.Minus: return left - right;
            }
            return 0;
        }

        public double VisitGrouping(Grouping expr)
        {
            return expr.Expression.Accept(this);
        }

        public double VisitLiteral(Literal expr)
        {
            if (expr.Value == null) return 0.0;
            return Convert.ToDouble(expr.Value);
        }

        public double VisitUnary(Unary expr)
        {
            throw new NotImplementedException();
        }
    }
    public class RpnPrinter : IVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }
        public string VisitBinary(Binary expr)
        {
            return $"{expr.Left.Accept(this)} {expr.Right.Accept(this)} {expr.Op.Lexeme}";           

        }

        public string VisitGrouping(Grouping expr)
        {
            return expr.Expression.Accept(this);
        }

        public string VisitLiteral(Literal expr)
        {
            if (expr.Value == null) return "nil" ;
            return expr.Value.ToString();
        }

        public string VisitUnary(Unary expr)
        {
            throw new NotImplementedException();
        }
    }
}
