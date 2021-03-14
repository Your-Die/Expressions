namespace Chinchillada.Expressions
{
    public class VoidLog : ILog
    {
        public void Error(int line, string message)
        {
        }

        public void Error(Token token, string errorMessage)
        {
        }

        public void RuntimeError(RuntimeError error)
        {
        }
    }
}