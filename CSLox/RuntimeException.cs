﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CSLox
{

    [Serializable]
    public class RuntimeException : Exception
    {
        public Token Token { get; private set; }
        public RuntimeException() { }
        public RuntimeException(string message, Token token) : base(message) { this.Token = token; }
        public RuntimeException(string message, Token token, Exception inner) : base(message, inner) { this.Token = token; }
        protected RuntimeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class Return : Exception
    {
        public object Value { get; private set; }
        public Return(object value)
        {
            this.Value = value;
        }
    }

    public class BreakException : Exception { }
    public class ContinueException : Exception { }
}
