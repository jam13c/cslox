using System;
using System.IO;

namespace CSLox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Runtime.Writer = Console.Out;
            if (args.Length > 1)
            {
                Runtime.Writer.WriteLine("Usage: cslox [script]");
            }
            else if (args.Length == 1)
            {
                Runtime.RunFile(args[0]);
            }
            else
            {
                Runtime.RunPrompt();
            }

        }

        
    }
}
