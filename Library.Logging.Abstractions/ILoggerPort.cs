namespace Library.Logging.Abstractions
{
    public interface ILoggerPort
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message, Exception? ex = null);
    }
}
