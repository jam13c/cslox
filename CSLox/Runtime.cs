using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSLox
{
    public class Runtime
    {
        public static TextWriter Writer = Console.Out;
        public static TextWriter Errors = Console.Error;
        public static TextReader Reader = Console.In;
        static Interpreter interpreter = new Interpreter();
        static bool hadError = false;
        static bool hadRuntimeError = false;

        public static void RunFile(string path)
        {
            var text = File.ReadAllText(path);
            Run(text);
        }

        public static void RunPrompt()
        {
            while (true)
            {
                Writer.Write(">");
                Run(Reader.ReadLine());
                hadError = false;
            }
        }

        static void Run(string source)
        {
            hadError = false;
            hadRuntimeError = false;
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            var statements = parser.Parse();
            if (hadError) return;

            if (statements.Count == 1 && statements[0] is Stmt.Expression expr)
                statements = new List<Stmt> { new Stmt.Print(expr.Expr) };

            var resolver = new Resolver(interpreter);
            resolver.Resolve(statements);
            if (hadError) return;

            interpreter.Interpret(statements);
        }
        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }
        public static void Error(Token token, string message)
        {
            hadError = true;
            if (token.Type == TokenType.EOF)
                Report(token.Line, " at end", message);
            else
                Report(token.Line, $" at '{token.Lexeme}'", message);
        }

        public static void RuntimeError(RuntimeException ex)
        {
            Errors.WriteLine($"[Line {ex.Token.Line}] {ex.Message}");
            hadRuntimeError = true;
        }

        static void Report(int line, string where, string message)
        {
            Errors.WriteLine($"[Line {line}] Error{where}: {message}");
        }

        //public static void Warn(string message)
        //{
        //    Writer.WriteLine(message);
        //}

    }
}
