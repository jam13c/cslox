using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public class Function : ICallable
    {
        private readonly Stmt.Function declaration;
        private readonly Environment closure;
        private readonly bool isInitializer;
        public Function(Stmt.Function declaration, Environment closure, bool isInitializer)
        {
            this.declaration = declaration;
            this.closure = closure;
            this.isInitializer = isInitializer;
        }
        public int Arity() => declaration.Parms.Count;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new Environment(closure);
            for (var i = 0; i < declaration.Parms.Count; i++)
                environment.Define(declaration.Parms[i].Lexeme, arguments[i]);

            try
            {
                interpreter.ExecuteBlock(declaration.Body, environment);
            }
            catch(Return returnValue)
            {
                if (isInitializer)
                    return closure.GetAt(0, "this");
                return returnValue.Value;
            }

            if (isInitializer)
                return closure.GetAt(0, "this");
            return null;
        }

        public Function Bind(Instance instance)
        {
            var environment = new Environment(closure);
            environment.Define("this", instance);
            return new Function(declaration, environment, isInitializer);
        }

        public override string ToString() => $"<fn {declaration.Name.Lexeme}>";
    }
}
