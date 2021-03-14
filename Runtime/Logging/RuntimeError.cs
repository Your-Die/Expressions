namespace Chinchillada.Expressions
{
    using System;

    [Serializable]
    public class RuntimeError : Exception
    {
        public Token Token { get; }

        public RuntimeError(Token token, string message) : base(message)
        {
            this.Token = token;
        }
    }
}