using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public class Environment
    {
        private readonly Environment enclosing;
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        public Environment(Environment enclosing = null)
        {
            this.enclosing = enclosing;
        }
        public Environment Enclosing => enclosing;

        public void Define(string name, object value) => values[name] = value;

        public object Get(Token name)
        {
            if (values.TryGetValue(name.Lexeme, out var value))
            {
                return value;
            }
            if (enclosing != null)
                return enclosing.Get(name);

            throw new RuntimeException($"Undefined variable '{name.Lexeme}'", name);
            
        }

        public object GetAt(int distance, string name)
        {
            return Ancestor(distance).values[name];
        }

        public void Assign(Token name, object value)
        {
            if(values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
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
            Ancestor(distance).values[name.Lexeme] = value;
        }

        private Environment Ancestor(int distance)
        {
            var environment = this;
            for (var i = 0; i < distance; i++)
                environment = environment.enclosing;
            return environment;
        }

    }
}
