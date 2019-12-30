using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while(!IsAtEnd())
            {
                statements.Add(Declaration());
            }
            return statements;
        }

        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.Class)) return ClassDeclaration();
                if (Match(TokenType.Fun)) return Function("function");
                if (Match(TokenType.Var)) return VarDeclaration();

                return Statement();
            }
            catch(ParseException ex)
            {
                Synchronize();
                return null;
            }
        }

        

        private Stmt Statement()
        {
            if (Match(TokenType.For)) return ForStatement();
            if (Match(TokenType.If)) return IfStatement();
            if (Match(TokenType.Print)) return PrintStatement();
            if (Match(TokenType.Return)) return ReturnStatement();
            if (Match(TokenType.While)) return WhileStatement();
            if (Match(TokenType.LeftBrace)) return new Stmt.Block(Block());

            return ExpressionStatement();
        }

        private Stmt ForStatement()
        {
            Consume(TokenType.LeftParen, "Expect '(' after 'for'");
            Stmt initializer = null;
            if (Match(TokenType.Semicolon))
                initializer = null;
            else if (Match(TokenType.Var))
                initializer = VarDeclaration();
            else
                initializer = ExpressionStatement();

            Expr condition = null;
            if (!Check(TokenType.Semicolon))
                condition = Expression();
            Consume(TokenType.Semicolon, "Expect ';' after loop condition");

            Expr increment = null;
            if (!Check(TokenType.RightParen))
                increment = Expression();
            Consume(TokenType.RightParen, "Expect ')' after for clauses");

            var body = Statement();

            if (increment != null)
                body = new Stmt.Block(new List<Stmt>
                {
                    body,
                    new Stmt.Expression(increment)
                });

            if (condition == null)
                condition = new Expr.Literal(true);
            body = new Stmt.While(condition, body);

            if (initializer != null)
                body = new Stmt.Block(new List<Stmt>
                {
                    initializer,
                    body
                });

            return body;
        }

        private Stmt IfStatement()
        {
            Consume(TokenType.LeftParen, "Expect '(' after 'if'");
            var condition = Expression();
            Consume(TokenType.RightParen, "Expect ')' after if condition");

            var thenBranch = Statement();
            var elseBranch = Match(TokenType.Else) ? Statement() : null;
            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        private Stmt PrintStatement()
        {
            var expr = Expression();
            Consume(TokenType.Semicolon, "Expect ';' after value");
            return new Stmt.Print(expr);
        }

        private Stmt ReturnStatement()
        {
            var keyword = Previous();
            var value = !Check(TokenType.Semicolon) ? Expression() : null;
            Consume(TokenType.Semicolon, "Expect ';' after return value");
            return new Stmt.Return(keyword, value);
        }

        private Stmt WhileStatement()
        {
            Consume(TokenType.LeftParen, "Expect '(' after 'while'");
            var condition = Expression();
            Consume(TokenType.RightParen, "Expect ')' after condition");
            var body = Statement();

            return new Stmt.While(condition, body);
        }        
        private Stmt ExpressionStatement()
        {
            var expr = Expression();
            Consume(TokenType.Semicolon, "Expect ';' after value");
            return new Stmt.Expression(expr);
        }

        private Stmt ClassDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expect class name");

            Expr.Variable superclass = null;
            if(Match(TokenType.Less))
            {
                Consume(TokenType.Identifier, "Expect superclass name");
                superclass = new Expr.Variable(Previous());
            }

            Consume(TokenType.LeftBrace, "Expect '{' after class name");
            var methods = new List<Stmt.Function>();
            while (!Check(TokenType.RightBrace) && !IsAtEnd())
                methods.Add(Function("method"));
            Consume(TokenType.RightBrace, "Expect '}' after class body");
            return new Stmt.Class(name, superclass, methods);
        }

        private Stmt.Function Function(string kind)
        {
            var name = Consume(TokenType.Identifier, $"Expect {kind} name");
            Consume(TokenType.LeftParen, $"Expect '(' after {kind} name");
            var parameters = new List<Token>();
            if(!Check(TokenType.RightParen))
            {
                do
                {
                    if (parameters.Count >= 255)
                    {
                        Error(Peek(), "Cannot have more than 255 parameters");
                    }
                    parameters.Add(Consume(TokenType.Identifier, "Expect parameter name"));
                }
                while (Match(TokenType.Comma));
            }
            Consume(TokenType.RightParen, "Expect ')' after parameters");
            Consume(TokenType.LeftBrace, $"Expect '{{' before {kind} body");
            var body = Block();
            return new Stmt.Function(name, parameters, body);
        }

        private Stmt VarDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expect varaible name");

            Expr initializer = null;
            if(Match(TokenType.Equal))
            {
                initializer = Expression();
            }
            Consume(TokenType.Semicolon, "Expect ';' after variable declaration");
            return new Stmt.Var(name, initializer);
        }

        
        private List<Stmt> Block()
        {
            var statements = new List<Stmt>();
            while (!Check(TokenType.RightBrace) && !IsAtEnd())
                statements.Add(Declaration());
            Consume(TokenType.RightBrace, "Expect '}' after block");
            return statements;
        }

        private Expr Expression()
        {
            return Comma();
        }

        private Expr Comma()
        {
            var expr = Ternary();
            if(Match(TokenType.Comma))
            {
                var op = Previous();
                var right = Expression();
                return new Expr.Comma(expr, op, right);
            }
            return expr;
        }

        private Expr Ternary()
        {
            var expr = Assignment();
            if(Match(TokenType.QuestionMark))
            {
                var trueExpr = Expression();
                Consume(TokenType.Colon, "Expect ':' after expression");
                var falseExpr = Expression();
                return new Expr.Ternary(expr, trueExpr, falseExpr);
            }
            return expr;
        }
        private Expr Assignment()
        {
            var expr = Or();
            if (Match(TokenType.Equal))
            {
                var equals = Previous();
                var value = Assignment();
                if (expr is Expr.Variable variableExpr)
                {
                    var name = variableExpr.Name;
                    return new Expr.Assign(name, value);
                }
                else if(expr is Expr.Get getExpr)
                {
                    return new Expr.Set(getExpr.Obj, getExpr.Name, value);
                }
                Error(equals, "Invalid assignment target");
            }
            return expr;
        }

        private Expr Or()
        {
            var expr = And();
            while(Match(TokenType.Or))
            {
                var op = Previous();
                var right = And();
                expr = new Expr.Logical(expr, op, right);
            }
            return expr;
        }

        private Expr And()
        {
            var expr = Equality();
            while (Match(TokenType.And))
            {
                var op = Previous();
                var right = Equality();
                expr = new Expr.Logical(expr, op, right);
            }
            return expr;
        }

        private Expr Equality()
        {
            var expr = Comparison();
            while(Match(TokenType.BangEqual, TokenType.EqualEqual))
            {
                var op = Previous();
                var right = Comparison();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        private Expr Comparison()
        {
            var expr = Addition();
            while(Match(TokenType.Greater,TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
            {
                var op = Previous();
                var right = Addition();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        private Expr Addition()
        {
            var expr = Multipication();
            while (Match(TokenType.Minus, TokenType.Plus))
            {
                var op = Previous();
                var right = Multipication();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        private Expr Multipication()
        {
            var expr = Unary();
            while (Match(TokenType.Slash, TokenType.Star))
            {
                var op = Previous();
                var right = Unary();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        private Expr Unary()
        {
            if(Match(TokenType.Bang, TokenType.Minus))
            {
                var op = Previous();
                var right = Unary();
                return new Expr.Unary(op, right);
            }

            return Call();
        }

        private Expr Call()
        {
            var expr = Primary();
            while (true)
            {
                if (Match(TokenType.LeftParen))
                    expr = FinishCall(expr);
                else if (Match(TokenType.Dot))
                {
                    var name = Consume(TokenType.Identifier, "Expect property name afrter '.'");
                    expr = new Expr.Get(expr, name);
                }
                else
                    break;
            }
            return expr;
        }

        private Expr FinishCall(Expr callee)
        {
            var args = new List<Expr>();
            if(!Check(TokenType.RightParen))
            {
                do
                {
                    args.Add(Expression());
                } while (Match(TokenType.Comma));
            }
            if (args.Count >= 255)
                Error(Peek(), "Cannot have more than 255 arguments");

            var paren = Consume(TokenType.RightParen, "Expect ')' after arguments");
            return new Expr.Call(callee, paren, args);
        }

        private Expr Primary()
        {
            if (Match(TokenType.False)) return new Expr.Literal(false);
            if (Match(TokenType.True)) return new Expr.Literal(true);
            if (Match(TokenType.Nil)) return new Expr.Literal(null);

            if (Match(TokenType.Number, TokenType.String))
                return new Expr.Literal(Previous().Literal);

            if(Match(TokenType.Super))
            {
                var keyword = Previous();
                Consume(TokenType.Dot, "Expect '.' after super");
                var method = Consume(TokenType.Identifier, "Expect superclass method name");
                return new Expr.Super(keyword, method);
            }

            if (Match(TokenType.This)) return new Expr.This(Previous());

            if (Match(TokenType.Identifier))
                return new Expr.Variable(Previous());

            if(Match(TokenType.LeftParen))
            {
                var expr = Expression();
                Consume(TokenType.RightParen, "Expect ')' after expression");
                return new Expr.Grouping(expr);
            }
            if(Match(TokenType.Fun))
            {
                return AnonymousFunction();
            }

            throw Error(Peek(), "Expect expression");
        }

        private Expr AnonymousFunction()
        {
            Consume(TokenType.LeftParen, "Expect '(' after fun");
            var parameters = new List<Token>();
            if (!Check(TokenType.RightParen))
            {
                do
                {
                    if (parameters.Count >= 255)
                    {
                        Error(Peek(), "Cannot have more than 255 parameters");
                    }
                    parameters.Add(Consume(TokenType.Identifier, "Expect parameter name"));
                }
                while (Match(TokenType.Comma));
            }
            Consume(TokenType.RightParen, "Expect ')' after parameters");
            Consume(TokenType.LeftBrace, "Expect '{' before body");
            var body = Block();
            return new Expr.AnonymousFunction(parameters, body);
        }

        private bool Match(params TokenType [] types)
        {
            foreach(var type in types)
            {
                if(Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }
        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();

            throw Error(Peek(), message);
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        private bool IsAtEnd() => Peek().Type == TokenType.EOF;
        private Token Peek() => tokens[current];
        private Token Previous() => tokens[current - 1];

        private ParseException Error(Token token, string message)
        {
            Runtime.Error(token, message);
            return new ParseException(message);
        }

        private void Synchronize()
        {
            Advance();
            while(!IsAtEnd())
            {
                if (Previous().Type == TokenType.Semicolon) return;

                switch(Peek().Type)
                {
                    case TokenType.Class:
                    case TokenType.Fun:
                    case TokenType.Var:
                    case TokenType.For:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Print:
                    case TokenType.Return:
                        return;
                }
                Advance();
            }
        }
    }
}
