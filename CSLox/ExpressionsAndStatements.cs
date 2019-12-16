using System;

namespace CSLox 
{
	public interface IExprVisitor<T>
    {
        T VisitBinaryExpr(Expr.Binary expr);
        T VisitGroupingExpr(Expr.Grouping expr);
        T VisitLiteralExpr(Expr.Literal expr);
        T VisitUnaryExpr(Expr.Unary expr);
    }

	public interface IStmtVisitor
    {
        void VisitExpressionStmt(Stmt.Expression stmt);
        void VisitPrintStmt(Stmt.Print stmt);
    }

	public abstract class Expr
	{
		public abstract T Accept<T>(IExprVisitor<T> visitor);

		public class Binary : Expr
		{
			public Expr Left { get; private set;}
			public Token Op { get; private set;}
			public Expr Right { get; private set;}
			public Binary(Expr left, Token op, Expr right)
			{
				this.Left = left;
				this.Op = op;
				this.Right = right;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitBinaryExpr(this);
		}
		public class Grouping : Expr
		{
			public Expr Expression { get; private set;}
			public Grouping(Expr expression)
			{
				this.Expression = expression;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitGroupingExpr(this);
		}
		public class Literal : Expr
		{
			public Object Value { get; private set;}
			public Literal(Object value)
			{
				this.Value = value;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitLiteralExpr(this);
		}
		public class Unary : Expr
		{
			public Token Op { get; private set;}
			public Expr Right { get; private set;}
			public Unary(Token op, Expr right)
			{
				this.Op = op;
				this.Right = right;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitUnaryExpr(this);
		}
	}

	public abstract class Stmt
	{
		public abstract void Accept(IStmtVisitor visitor);

		public class Expression : Stmt
		{
			public Expr Expr { get; private set;}
			public Expression(Expr expr)
			{
				this.Expr = expr;
			}
			public override void Accept(IStmtVisitor visitor) => visitor.VisitExpressionStmt(this);
		}
		public class Print : Stmt
		{
			public Expr Expr { get; private set;}
			public Print(Expr expr)
			{
				this.Expr = expr;
			}
			public override void Accept(IStmtVisitor visitor) => visitor.VisitPrintStmt(this);
		}
	
	}

}



