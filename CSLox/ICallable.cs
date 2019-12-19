using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public interface ICallable
    {
        int Arity();
        object Call(Interpreter interpreter, List<object> arguments);
    }
}
