using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public class Function : ICallable
    {
        private readonly Stmt.Function declaration;
        public Function(Stmt.Function declaration)
        {
            this.declaration = declaration;
        }
        public int Arity() => declaration.Parms.Count;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new Environment(interpreter.Globals);
            for (var i = 0; i < declaration.Parms.Count; i++)
                environment.Define(declaration.Parms[i].Lexeme, arguments[i]);

            try
            {
                interpreter.ExecuteBlock(declaration.Body, environment);
                return null;
            }
            catch(Return returnValue)
            {
                return returnValue.Value;
            }
        }

        public override string ToString() => $"<fn {declaration.Name.Lexeme}>";
    }
}
