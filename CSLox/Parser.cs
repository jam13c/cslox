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
                statements.Add(Statement());
            }
            return statements;
        }

        private Stmt Statement()
        {
            if (Match(TokenType.Print)) return PrintStatement();
            return ExpressionStatement();
        }

        
        private Stmt PrintStatement()
        {
            var expr = Expression();
            Consume(TokenType.Semicolon, "Expect ';' after value");
            return new Stmt.Print(expr);
        }

        private Stmt ExpressionStatement()
        {
            var expr = Expression();
            Consume(TokenType.Semicolon, "Expect ';' after value");
            return new Stmt.Expression(expr);
        }


        private Expr Expression()
        {
            return Equality();
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
            return Primary();
        }

        private Expr Primary()
        {
            if (Match(TokenType.False)) return new Expr.Literal(false);
            if (Match(TokenType.True)) return new Expr.Literal(true);
            if (Match(TokenType.Nil)) return new Expr.Literal(null);

            if (Match(TokenType.Number, TokenType.String))
                return new Expr.Literal(Previous().Literal);
            if(Match(TokenType.LeftParen))
            {
                var expr = Expression();
                Consume(TokenType.RightParen, "Expect ')' after expression");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expect expression");
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
            Program.Error(token, message);
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
