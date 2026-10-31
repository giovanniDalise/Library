using Library.Logging.Abstractions;

namespace Library.Logging.NLog;

public class NLogAdapter<T> : NLogAdapter, ILoggerPort<T>
{
    public NLogAdapter() : base(typeof(T).FullName!) { }
}