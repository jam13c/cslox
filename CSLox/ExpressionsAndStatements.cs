using System;
using System.Collections.Generic;

namespace CSLox 
{
	public interface IExprVisitor<T>
    {
        T VisitAnonymousFunctionExpr(Expr.AnonymousFunction expr);
        T VisitAssignExpr(Expr.Assign expr);
        T VisitBinaryExpr(Expr.Binary expr);
        T VisitCallExpr(Expr.Call expr);
        T VisitCommaExpr(Expr.Comma expr);
        T VisitGetExpr(Expr.Get expr);
        T VisitGroupingExpr(Expr.Grouping expr);
        T VisitLiteralExpr(Expr.Literal expr);
        T VisitLogicalExpr(Expr.Logical expr);
        T VisitSetExpr(Expr.Set expr);
        T VisitSuperExpr(Expr.Super expr);
        T VisitTernaryExpr(Expr.Ternary expr);
        T VisitThisExpr(Expr.This expr);
        T VisitUnaryExpr(Expr.Unary expr);
        T VisitVariableExpr(Expr.Variable expr);
    }

	public interface IStmtVisitor
    {
        void VisitBlockStmt(Stmt.Block stmt);
        void VisitClassStmt(Stmt.Class stmt);
        void VisitExpressionStmt(Stmt.Expression stmt);
        void VisitFunctionStmt(Stmt.Function stmt);
        void VisitIfStmt(Stmt.If stmt);
        void VisitPrintStmt(Stmt.Print stmt);
        void VisitReturnStmt(Stmt.Return stmt);
        void VisitVarStmt(Stmt.Var stmt);
        void VisitWhileStmt(Stmt.While stmt);
    }

	public abstract class Expr
	{
		public abstract T Accept<T>(IExprVisitor<T> visitor);

		public class AnonymousFunction : Expr
		{
			public List<Token> Parms { get; private set;}
			public List<Stmt> Body { get; private set;}
			public AnonymousFunction(List<Token> parms, List<Stmt> body)
			{
				this.Parms = parms;
				this.Body = body;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitAnonymousFunctionExpr(this);
		}
		public class Assign : Expr
		{
			public Token Name { get; private set;}
			public Expr Value { get; private set;}
			public Assign(Token name, Expr value)
			{
				this.Name = name;
				this.Value = value;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitAssignExpr(this);
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
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitBinaryExpr(this);
		}
		public class Call : Expr
		{
			public Expr Callee { get; private set;}
			public Token Paren { get; private set;}
			public List<Expr> Arguments { get; private set;}
			public Call(Expr callee, Token paren, List<Expr> arguments)
			{
				this.Callee = callee;
				this.Paren = paren;
				this.Arguments = arguments;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitCallExpr(this);
		}
		public class Comma : Expr
		{
			public Expr Left { get; private set;}
			public Token Op { get; private set;}
			public Expr Right { get; private set;}
			public Comma(Expr left, Token op, Expr right)
			{
				this.Left = left;
				this.Op = op;
				this.Right = right;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitCommaExpr(this);
		}
		public class Get : Expr
		{
			public Expr Obj { get; private set;}
			public Token Name { get; private set;}
			public Get(Expr obj, Token name)
			{
				this.Obj = obj;
				this.Name = name;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitGetExpr(this);
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
		public class Logical : Expr
		{
			public Expr Left { get; private set;}
			public Token Op { get; private set;}
			public Expr Right { get; private set;}
			public Logical(Expr left, Token op, Expr right)
			{
				this.Left = left;
				this.Op = op;
				this.Right = right;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitLogicalExpr(this);
		}
		public class Set : Expr
		{
			public Expr Obj { get; private set;}
			public Token Name { get; private set;}
			public Expr Value { get; private set;}
			public Set(Expr obj, Token name, Expr value)
			{
				this.Obj = obj;
				this.Name = name;
				this.Value = value;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitSetExpr(this);
		}
		public class Super : Expr
		{
			public Token Keyword { get; private set;}
			public Token Method { get; private set;}
			public Super(Token keyword, Token method)
			{
				this.Keyword = keyword;
				this.Method = method;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitSuperExpr(this);
		}
		public class Ternary : Expr
		{
			public Expr Condition { get; private set;}
			public Expr TrueExpr { get; private set;}
			public Expr FalseExpr { get; private set;}
			public Ternary(Expr condition, Expr trueExpr, Expr falseExpr)
			{
				this.Condition = condition;
				this.TrueExpr = trueExpr;
				this.FalseExpr = falseExpr;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitTernaryExpr(this);
		}
		public class This : Expr
		{
			public Token Keyword { get; private set;}
			public This(Token keyword)
			{
				this.Keyword = keyword;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitThisExpr(this);
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
		public class Variable : Expr
		{
			public Token Name { get; private set;}
			public Variable(Token name)
			{
				this.Name = name;
			}
			public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitVariableExpr(this);
		}
	}

	public abstract class Stmt
	{
		public abstract void Accept(IStmtVisitor visitor);

		public class Block : Stmt
		{
			public List<Stmt> Statements { get; private set;}
			public Block(List<Stmt> statements)
			{
				this.Statements = statements;
			}
			public override void Accept(IStmtVisitor visitor) => visitor.VisitBlockStmt(this);
		}
		public class Class : Stmt
		{
			public Token Name { get; private set;}
			public Expr.Variable Superclass { get; private set;}
			public List<Stmt.Function> Methods { get; private set;}
			public Class(Token name, Expr.Variable superclass, List<Stmt.Function> methods)
			{
				this.Name = name;
				this.Superclass = superclass;
				this.Methods = methods;
			}
			public override void Accept(IStmtVisitor visitor) => visitor.VisitClassStmt(this);
		}
		public class Expression : Stmt
		{
			public Expr Expr { get; private set;}
			public Expression(Expr expr)
			{
				this.Expr = expr;
			}
			public override void Accept(IStmtVisitor visitor) => visitor.VisitExpressionStmt(this);
		}
		public class Function : Stmt
		{
			public Token Name { get; private set;}
			public List<Token> Parms { get; private set;}
			public List<Stmt> Body { get; private set;}
			public Function(Token name, List<Token> parms, List<Stmt> body)
			{
				this.Name = name;
				this.Parms = parms;
				this.Body = body;
			}
			public override void Accept(IStmtVisitor visitor) => visitor.VisitFunctionStmt(this);
		}
		public class If : Stmt
		{
			public Expr Condition { get; private set;}
			public Stmt ThenBranch { get; private set;}
			public Stmt ElseBranch { get; private set;}
			public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
			{
				this.Condition = condition;
				this.ThenBranch = thenBranch;
				this.ElseBranch = elseBranch;
			}
			public override void Accept(IStmtVisitor visitor) => visitor.VisitIfStmt(this);
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
		public class Return : Stmt
		{
			public Token Keyword { get; private set;}
			public Expr Value { get; private set;}
			public Return(Token keyword, Expr value)
			{
				this.Keyword = keyword;
				this.Value = value;
			}
			public override void Accept(IStmtVisitor visitor) => visitor.VisitReturnStmt(this);
		}
		public class Var : Stmt
		{
			public Token Name { get; private set;}
			public Expr Initializer { get; private set;}
			public Var(Token name, Expr initializer)
			{
				this.Name = name;
				this.Initializer = initializer;
			}
			public override void Accept(IStmtVisitor visitor) => visitor.VisitVarStmt(this);
		}
		public class While : Stmt
		{
			public Expr Condition { get; private set;}
			public Stmt Body { get; private set;}
			public While(Expr condition, Stmt body)
			{
				this.Condition = condition;
				this.Body = body;
			}
			public override void Accept(IStmtVisitor visitor) => visitor.VisitWhileStmt(this);
		}
	
	}

}



