using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{
    public class Instance
    {
        private readonly Class cls;
        private readonly Dictionary<string, object> fields = new Dictionary<string, object>();
        public Instance(Class cls)
        {
            this.cls = cls;
        }

        public object Get(Token name)
        {
            if (fields.TryGetValue(name.Lexeme, out var value))
                return value;

            var getter = cls.FindMethod($"get_{name.Lexeme}");
            if (getter != null)
                return getter.Bind(this);

            var method = cls.FindMethod(name.Lexeme);
            if (method != null)
                return method.Bind(this).Call(null,null);

            throw new RuntimeException($"Undefined property '{name.Lexeme}'", name);
        }
        public void Set(Token name, object value)
        {
            fields[name.Lexeme] = value;
        }

        public override string ToString() => $"{cls} instance";

    }
}
