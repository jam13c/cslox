using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public class Token
    {
        public TokenType Type { get; private set; }
        public string Lexeme { get; private set; }
        public object Literal { get; private set; }
        public int Line { get; private set; }

        public Token(TokenType type, string lexeme, object literal, int line)
        {
            this.Type = type;
            this.Lexeme = lexeme;
            this.Literal = literal;
            this.Line = line;
        }

        public override string ToString() => $"{Type} {Lexeme} {Literal}";

    }

    public enum TokenType
    {
        // Single char
        LeftParen, RightParen, LeftBrace, RightBrace,Comma,Dot,Minus,Plus,Semicolon,Slash,Star, QuestionMark, Colon,

        // one or two char
        Bang, BangEqual, Equal,EqualEqual,Greater,GreaterEqual,Less,LessEqual,

        // Literals
        Identifier, String, Number,

        // Keywords
        And,Class,Else,False,Fun,For,If,Nil,Or,Print,Return,Super,This,True,Var,While,

        EOF

    }
}
