namespace Chinchillada.Expressions
{
    using UnityEngine;

    public class UnityLogger : ILog
    {
        public void Error(int line, string message)
        {
            Report(line, string.Empty, message);
        }

        public void Error(Token token, string errorMessage)
        {
            var where = token.Type == TokenType.EOF
                ? " at end"
                : $" at '{token.Lexeme}'";

            Report(token.Line, where, errorMessage);
        }

        public void RuntimeError(RuntimeError error)
        {
            Debug.LogError($"{error.Message}\n[line {error.Token.Line}]");
        }

        private static void Report(int line, string where, string message)
        {
            Debug.LogError($"[line {line}] {where}: {message}");
        }
    }
}