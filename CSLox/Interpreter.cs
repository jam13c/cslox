using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public class Interpreter : IExprVisitor<object>, IStmtVisitor
    {
        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach(var stmt in statements)
                {
                    Execute(stmt);
                }
            }
            catch(RuntimeException ex)
            {
                Program.RuntimeError(ex);
            }
        }


        public object VisitBinaryExpr(Expr.Binary expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            switch(expr.Op.Type)
            {
                case TokenType.Greater:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left > (double)right;
                case TokenType.GreaterEqual:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left >= (double)right;
                case TokenType.Less:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left < (double)right;
                case TokenType.LessEqual:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left <= (double)right;
                case TokenType.Minus:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left - (double)right;
                case TokenType.Plus when left is double leftDouble && right is double rightDouble:
                    return leftDouble + rightDouble;
                case TokenType.Plus when left is string leftString && right is string rightString:
                    return leftString + rightString;
                case TokenType.Plus when left is string leftString && right is double rightDouble:
                    return leftString + rightDouble;
                case TokenType.Plus when left is double leftDouble && right is string rightString:
                    return leftDouble + rightString;
                case TokenType.Plus:
                    throw InvalidOperator("Operands must be two numbers or two strings or string and number", expr.Op);
                case TokenType.Slash:
                    CheckNumberOperands(expr.Op, left, right);
                    CheckNonZero(expr.Op, right);
                    return (double)left / (double)right;
                case TokenType.Star:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left * (double)right;
                case TokenType.BangEqual:
                    return !IsEqual(left, right);
                case TokenType.EqualEqual:
                    return IsEqual(left, right);
            }

            return null;
        }

        public object VisitGroupingExpr(Expr.Grouping expr) => Evaluate(expr.Expression);

        public object VisitLiteralExpr(Expr.Literal expr) => expr.Value;

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            var right = Evaluate(expr.Right);
            switch(expr.Op.Type)
            {
                case TokenType.Bang:
                    return !IsTruthy(right);
                case TokenType.Minus:
                    CheckNumberOperand(expr.Op, right);
                    return -(double)right;
            }
            return null;
        }
        public void VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.Expr);
        }

        public void VisitPrintStmt(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.Expr);
            Console.WriteLine(Stringify(value));
        }

        private Object Evaluate(Expr expr) => expr.Accept(this);
        private void Execute(Stmt stmt) => stmt.Accept(this);

        private bool IsTruthy(object value)
        {
            if (value == null) return false;
            if (value is bool boolValue) return boolValue;
            return true;
        }

        private bool IsEqual(object left, object right)
        {
            if(left == null && right == null) return true;
            if (left == null) return false;
            return left.Equals(right);
        }

        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is double) return;

            throw new RuntimeException("Operand must be a number", op);
        }
        private void CheckNumberOperands(Token op, object left, object right)
        {
            if (left is double && right is double) return;

            throw new RuntimeException("Operands must be a numbers", op);
        }

        private void CheckNonZero(Token op, object value)
        {
            if (value is double doubleValue && doubleValue != 0) return;

            throw new RuntimeException("Value cannot be zero", op);
        }

        private RuntimeException InvalidOperator(string message, Token op)
        {
            return new RuntimeException(message, op);
        }

        private string Stringify(object value)
        {
            if (value == null) return "nil";
            return value.ToString();
        }

        

    }
}
