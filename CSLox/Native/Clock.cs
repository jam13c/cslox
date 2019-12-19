using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox.Native
{
    public class Clock : ICallable
    {
        public int Arity() => 0;

        public object Call(Interpreter interpreter, List<object> arguments) =>  (double)(DateTime.Now.Ticks/1000);

        public override string ToString() => "<native fn>";
    }
}
