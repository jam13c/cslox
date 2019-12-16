using System;

namespace CSLox 
{
	public interface IVisitor<T>
    {
        T VisitBinary(Binary expr);
        T VisitGrouping(Grouping expr);
        T VisitLiteral(Literal expr);
        T VisitUnary(Unary expr);
    }

	public abstract class Expr
	{
		public abstract T Accept<T>(IVisitor<T> visitor);
	}

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
		public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitBinary(this);
	}
	public class Grouping : Expr
	{
		public Expr Expression { get; private set;}
		public Grouping(Expr expression)
		{
			this.Expression = expression;
		}
		public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitGrouping(this);
	}
	public class Literal : Expr
	{
		public Object Value { get; private set;}
		public Literal(Object value)
		{
			this.Value = value;
		}
		public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitLiteral(this);
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
		public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitUnary(this);
	}
}

