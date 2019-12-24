﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{    
    public class Resolver : IExprVisitor<object>, IStmtVisitor
    {
        private enum FunctionType { None, Function, Method, Initializer }
        private enum ClassType { None, Class }

        private readonly Interpreter interpreter;
        private readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
        private FunctionType currentFunction = FunctionType.None;
        private ClassType currentClass = ClassType.None; 

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

        public void VisitClassStmt(Stmt.Class stmt)
        {
            var enclosingClass = currentClass;
            currentClass = ClassType.Class;
            Declare(stmt.Name);
            Define(stmt.Name);
            BeginScope();
            scopes.Peek().Add("this", true);
            foreach(var method in stmt.Methods)
            {
                var declaration = FunctionType.Method;
                if (method.Name.Lexeme == "init")
                    declaration = FunctionType.Initializer;
                ResolveFunction(method, declaration);
            }
            EndScope();
            currentClass = enclosingClass;
        }

        public void VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.Expr);
        }

        public void VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);
            ResolveFunction(stmt, FunctionType.Function);
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
            if(currentFunction == FunctionType.None)
            {
                Program.Error(stmt.Keyword, "Cannot return from top-level code");
            }
            if (stmt.Value != null)
            {
                if(currentFunction == FunctionType.Initializer)
                {
                    Program.Error(stmt.Keyword, "Cannot return value from an initializer");
                }
                Resolve(stmt.Value);
            }
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
            ResolveAnonymousFunction(expr);
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
        public object VisitGetExpr(Expr.Get expr)
        {
            Resolve(expr.Obj);
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

        public object VisitSetExpr(Expr.Set expr)
        {
            Resolve(expr.Value);
            Resolve(expr.Obj);
            return null;
        }

        public object VisitThisExpr(Expr.This expr)
        {
            if(currentClass == ClassType.None)
            {
                Program.Error(expr.Keyword, "Cannot use 'this' outside of a class");
                return null;
            }
            ResolveLocal(expr, expr.Keyword);
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
            for(var i=0;i<scopesArray.Length;i++)
            {
                if(scopesArray[i].ContainsKey(name.Lexeme))
                {
                    interpreter.Resolve(expr, i);
                    return;
                }
            }
        }
        private void ResolveFunction(Stmt.Function stmt, FunctionType functionType)
        {
            var enclosingFunction = currentFunction;
            currentFunction = functionType;
            BeginScope();
            foreach(var parm in stmt.Parms)
            {
                Declare(parm);
                Define(parm);
            }
            Resolve(stmt.Body);
            EndScope();
            currentFunction = enclosingFunction;
        }

        private void ResolveAnonymousFunction(Expr.AnonymousFunction expr)
        {
            BeginScope();
            foreach (var parm in expr.Parms)
            {
                Declare(parm);
                Define(parm);
            }
            Resolve(expr.Body);
            EndScope();
        }

        private void BeginScope()
        {
            scopes.Push(new Dictionary<string, bool>());
        }
        private void EndScope()
        {
            scopes.Pop();
        }
        private void Declare(Token name)
        {
            if (scopes.Count == 0) return;
            var scope = scopes.Peek();
            if(scope.ContainsKey(name.Lexeme))
            {
                Program.Error(name, "Variable with this name already declared in this scope");
            }
            scope[name.Lexeme] = false;
        }

        private void Define(Token name)
        {
            if (scopes.Count == 0) return;
            scopes.Peek()[name.Lexeme] = true;
        }

    }
}