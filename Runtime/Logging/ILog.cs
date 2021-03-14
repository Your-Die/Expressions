namespace Chinchillada.Expressions
{
    public interface ILog
    {
        void Error(int   line,  string message);
        void Error(Token token, string errorMessage);

        void RuntimeError(RuntimeError error);
    }
}