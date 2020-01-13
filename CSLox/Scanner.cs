using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSLox
{
    public class Scanner
    {
        private static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase)
        {
            ["and"] = TokenType.And,
            ["class"] = TokenType.Class,
            ["else"] = TokenType.Else,
            ["false"] = TokenType.False,
            ["for"] = TokenType.For,
            ["fun"] = TokenType.Fun,
            ["if"] = TokenType.If,
            ["nil"] = TokenType.Nil,
            ["or"] = TokenType.Or,
            ["print"] = TokenType.Print,
            ["return"] = TokenType.Return,
            ["super"] = TokenType.Super,
            ["this"] = TokenType.This,
            ["true"] = TokenType.True,
            ["var"] = TokenType.Var,
            ["while"] = TokenType.While,
            ["break"] = TokenType.Break,
            ["continue"] = TokenType.Continue,
            ["get"] = TokenType.Get,
            ["set"] = TokenType.Set
        };
        private readonly string source;
        private readonly List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;

        public Scanner(string source)
        {
            this.source = source;
        }

        public List<Token> ScanTokens()
        {
            while(!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, string.Empty, null, line));
            return tokens;
        }

        private bool IsAtEnd() => current >= source.Length;

        private void ScanToken()
        {
            var c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LeftParen);break;
                case ')': AddToken(TokenType.RightParen); break;
                case '{': AddToken(TokenType.LeftBrace); break;
                case '}': AddToken(TokenType.RightBrace); break;
                case ',': AddToken(TokenType.Comma); break;
                case '.': AddToken(TokenType.Dot); break;
                case '-': AddToken(TokenType.Minus); break;
                case '+': AddToken(TokenType.Plus); break;
                case ';': AddToken(TokenType.Semicolon); break;
                case '*': AddToken(TokenType.Star); break;
                case '?': AddToken(TokenType.QuestionMark); break;
                case ':': AddToken(TokenType.Colon); break;
                case '!': AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang); break;
                case '=': AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal); break;
                case '<': AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less); break;
                case '>': AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater); break;
                case '/' when Match('/'):
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                    break;
                case '/' when Match('*'):
                    BlockComment(line);
                    break;
                case '/' when !Match('/'):
                    AddToken(TokenType.Slash);
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break; // ignore whitespace
                case '\n':
                    line++;
                    break;
                case '"':
                    StringToken();break;

                default:
                    if (Char.IsDigit(c))
                    {
                        NumberToken();
                    }
                    else if(Char.IsLetter(c) || c == '_')
                    {
                        IdentifierToken();
                    }
                    else
                    {
                        Runtime.Error(line, $"Unexpected character '{c}'");
                    }
                    break;
            }
        }

        private void BlockComment(int line)
        {
            var nesting = 1;
            while (!IsAtEnd() && nesting > 0)
            {
                var curr = Peek();
                var next = PeekNext();
                if(curr == '/' && next == '*')
                {
                    Advance();
                    Advance();
                    nesting++;
                }
                else if(curr == '*' && next == '/')
                {
                    Advance();
                    Advance();
                    nesting--;
                }
                else
                {
                    Advance();
                }

            }

            if (nesting > 0 && IsAtEnd())
                Runtime.Error(line, "Unterminated block comment");
               
        }

        private void IdentifierToken()
        {
            while (Char.IsLetter(Peek()) || Char.IsDigit(Peek()) || Peek() == '_')
                Advance();
            var text = source.Substring(start, current - start);
            if (!keywords.TryGetValue(text, out var type))
                type = TokenType.Identifier;
            AddToken(type);

        }

        private void NumberToken()
        {
            while (Char.IsDigit(Peek()))
                Advance();
            if (Peek() == '.' && Char.IsDigit(PeekNext()))
                Advance(); // consume "."

            while (Char.IsDigit(Peek()))
                Advance();

            AddToken(TokenType.Number, double.Parse(source.Substring(start, current - start)));
        }

        private void StringToken()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }
            if(IsAtEnd())
            {
                Runtime.Error(line, "Unterminated string");
                return;
            }
            Advance();

            var value = source.Substring(start + 1, current - start - 2);
            AddToken(TokenType.String, value);
        }

        private char Advance()
        {
            current++;
            return source[current - 1];
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return source[current];
        }

        private char PeekNext()
        {
            if (current +1 >= source.Length) return '\0';
            return source[current+1];
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

        

        private void AddToken(TokenType type, object literal = null)
        {
            var text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}
