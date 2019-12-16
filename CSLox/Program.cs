using System;
using System.IO;

namespace CSLox
{
    class Program
    {
        static Interpreter interpreter = new Interpreter();
        static bool hadError = false;
        static bool hadRuntimeError = false;
        static void Main(string[] args)
        {

            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }

            //var expression = new Binary(
            //        new Grouping(
            //         new Binary(new Literal(1),new Token(TokenType.Plus,"+",null,1), new Literal(2))
            //        ),
            //        new Token(TokenType.Star, "*", null, 1),
            //        new Grouping(
            //         new Binary(new Grouping(new Binary( new Literal(20),new Token(TokenType.Slash,"/",null,1),new Literal(10)  )), new Token(TokenType.Minus, "-", null, 1), new Literal(3))
            //        )
            //    );

            //Console.WriteLine(new RpnCalculator().Evaluate(expression));

        }

        static void RunFile(string path)
        {
            var text = File.ReadAllText(path);
            Run(text);
        }

        static void RunPrompt()
        {
            while (true)
            {
                Console.Write(">");
                Run(Console.ReadLine());
                hadError = false;
            }
        }

        static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            var expr = parser.Parse();
            if (hadError) return;

            interpreter.Interpret(expr);
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
            Console.WriteLine(ex.Message);
            Console.WriteLine($"[line {ex.Token.Line}]");
            hadRuntimeError = true;
        }

        static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[Line {line}] Error{where}: {message}");
        }
    }
}
