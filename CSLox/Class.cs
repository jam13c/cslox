using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public class Class : ICallable
    {
        private readonly string name;
        private readonly Dictionary<string, Function> methods;

        public Class(string name, Dictionary<string,Function> methods)
        {
            this.name = name;
            this.methods = methods;
        }

        public int Arity()
        {
            var initializer = FindMethod("init");
            if (initializer == null) return 0;
            return initializer.Arity();
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var instance = new Instance(this);
            var initializer = FindMethod("init");
            if (initializer != null)
                initializer.Bind(instance).Call(interpreter, arguments);
            return instance;
        }

        public Function FindMethod(string name)
        {
            if (methods.TryGetValue(name, out var function))
                return function;

            return null;
        }

        public override string ToString() => this.name;
    }
}
