using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public class Environment
    {
        private readonly Environment enclosing;
        private readonly Dictionary<string, Value> values = new Dictionary<string, Value>();

        public Environment(Environment enclosing = null)
        {
            this.enclosing = enclosing;
        }
        public void Define(string name) => values[name] = new Value();
        public void Define(string name, object value) => values[name] = new Value(value);

        public object Get(Token name)
        {
            if (values.TryGetValue(name.Lexeme, out var value))
            {
                if (value.IsDefined)
                    return value.Instance;

                throw new RuntimeException($"Uninitialized variable '{name.Lexeme}'", name);
            }
            if (enclosing != null)
                return enclosing.Get(name);

            throw new RuntimeException($"Undefined variable '{name.Lexeme}'", name);
            
        }

        public object GetAt(int distance, Token name)
        {
            return Ancestor(distance).Get(name);
        }

        public void Assign(Token name, object value)
        {
            if(values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = new Value(value);
                return;
            }
            if(enclosing != null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeException($"Undefined variable '{name.Lexeme}'", name);
        }

        public void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance).Define(name.Lexeme, value);
        }

        private Environment Ancestor(int distance)
        {
            var environment = this;
            for (var i = 0; i < distance; i++)
                environment = environment.enclosing;
            return environment;
        }

        private class Value
        {
            public object Instance { get; private set; }
            public bool IsDefined { get; private set; }
            public Value(object instance)
            {
                this.Instance = instance;
                this.IsDefined = true;
            }

            public Value()
            {
                this.IsDefined = false;
            }
        }
    }
}
