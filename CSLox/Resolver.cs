using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public class Resolver : IExprVisitor<object>, IStmtVisitor
    {
        private readonly Interpreter interpreter;
        private readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }
        public void Resolve(List<Stmt> statements)
        {
            foreach (var stmt in statements)
                Resolve(stmt);
        }

        public void VisitBlockStmt(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.Statements);
            EndScope();
        }

        public void VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.Expr);
        }

        public void VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);
            ResolveFunction(stmt);
        }

       

        public void VisitIfStmt(Stmt.If stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.ThenBranch);
            if (stmt.ElseBranch != null) Resolve(stmt.ElseBranch);

        }
        public void VisitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt.Expr);
        }

        public void VisitReturnStmt(Stmt.Return stmt)
        {
            if (stmt.Value != null) Resolve(stmt.Value);
        }
        public void VisitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.Name);
            if(stmt.Initializer != null)
            {
                Resolve(stmt.Initializer);
            }
            Define(stmt.Name);
        }

        public void VisitWhileStmt(Stmt.While stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.Body);
        }

        public object VisitAnonymousFunctionExpr(Expr.AnonymousFunction expr)
        {
            return null;
        }

        public object VisitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);
            return null;
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        

        public object VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.Callee);
            foreach (var arg in expr.Arguments)
                Resolve(arg);
            return null;
        }


      

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.Expression);
            return null;
        }

       

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.Right);
            return null;
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            if(scopes.Count > 0 && (scopes.Peek().TryGetValue(expr.Name.Lexeme, out var isReady) && !isReady))
            {
                Program.Error(expr.Name, "Cannot read local variable in its own initialiser");
            }
            ResolveLocal(expr, expr.Name);
            return null;
        }
              

        

        private void Resolve(Stmt stmt) => stmt.Accept(this);
        private void Resolve(Expr expr) => expr.Accept(this);
        private void ResolveLocal(Expr expr, Token name)
        {
            var scopesArray = scopes.ToArray();
            for(var i= scopesArray.Length-1;i>=0;i--)
            {
                if(scopesArray[i].ContainsKey(name.Lexeme))
                {
                    interpreter.Resolve(expr, scopesArray.Length - 1 - i);
                    return;
                }
            }
        }
        private void ResolveFunction(Stmt.Function stmt)
        {
            BeginScope();
            foreach(var parm in stmt.Parms)
            {
                Declare(parm);
                Define(parm);
            }
            Resolve(stmt.Body);
            EndScope();
        }

        private void BeginScope() => scopes.Push(new Dictionary<string, bool>());

        private void EndScope() => scopes.Pop();

        private void Declare(Token name)
        {
            if (scopes.Count == 0) return;
            var scope = scopes.Peek();
            scope.Add(name.Lexeme, false);
        }

        private void Define(Token name)
        {
            if (scopes.Count == 0) return;
            scopes.Peek()[name.Lexeme] = true;
        }
    }
}
