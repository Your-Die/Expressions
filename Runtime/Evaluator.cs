namespace Chinchillada.Expressions
{
    public static class Evaluator
    {
        private static readonly UnityLogger Logger;
        private static readonly Interpreter Interpreter = new Interpreter(Logger);
        public static T Evaluate<T>(string source)
        {
            var scanner = new Scanner(source, Logger);
            var tokens  = scanner.ScanTokens();
            
            var parser     = new Parser(tokens, Logger);
            var expression = parser.Parse();

            var result = Interpreter.Interpret(expression);
            return (T) result;
        }
    }
}