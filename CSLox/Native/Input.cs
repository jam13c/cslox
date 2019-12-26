using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox.Native
{
    public class Input : ICallable
    {
        public int Arity() => 0;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return Console.ReadLine();
        }
    }
}
