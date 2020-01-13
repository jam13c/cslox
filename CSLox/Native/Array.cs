using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox.Native
{
    public class Array : ICallable
    {
        public int Arity() => 1;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var length = (double)arguments[0];
            return new Instance(interpreter, length);
        }

        public override string ToString() => "<array>";

        public class Instance 
        {
            private readonly Interpreter interpreter;
            private readonly object[] array;
            public Instance(Interpreter interpreter, double length)
            {
                this.interpreter = interpreter;
                this.array = new object[(int)length];
            }

            public object Get(Token name)
            {
                switch(name.Lexeme)
                {
                    case "length": return (double)array.Length;
                    case "get": return new GetMethod(array, name);
                    case "set": return new SetMethod(array, name);
                    case "copyto": return new CopyToMethod(array, name);
                }
                throw new RuntimeException($"Array does not support '{name.Lexeme}'", name);
            }

            private class GetMethod : ICallable
            {
                private readonly object[] array;
                private readonly Token name;
                public GetMethod(object[] array, Token name)
                {
                    this.array = array;
                    this.name = name;
                }
                public int Arity() => 1;

                public object Call(Interpreter interpreter, List<object> arguments)
                {
                    var index = (double)arguments[0];
                    if (index < 0 || index >= array.Length)
                        throw new RuntimeException("Index was outside of bounds", name);
                    return array[(int)index];
                }

                public override string ToString() => "<array get>";
            }

            private class SetMethod : ICallable
            {
                private readonly object[] array;
                private readonly Token name;
                public SetMethod(object[] array, Token name)
                {
                    this.array = array;
                    this.name = name;
                }
                public int Arity() => 2;

                public object Call(Interpreter interpreter, List<object> arguments)
                {
                    var index = (double)arguments[0];
                    var value = arguments[1];
                    if (index < 0 || index >= array.Length)
                        throw new RuntimeException("Index was outside of bounds", name);
                    array[(int)index] = value;
                    return null;
                }
                public override string ToString() => "<array set>";
            }

            private class CopyToMethod : ICallable
            {
                private readonly object[] array;
                private readonly Token name;
                public CopyToMethod(object[] array, Token name)
                {
                    this.array = array;
                    this.name = name;
                }
                public int Arity() => 1;

                public object Call(Interpreter interpreter, List<object> arguments)
                {
                    if(arguments[0] is Instance other)
                    {
                        array.CopyTo(other.array, 0);
                        return null;
                    }
                    throw new RuntimeException("copyto must take another array as argument", name);
                }
                public override string ToString() => "<array copyto>";
            }
        }

    }
}
