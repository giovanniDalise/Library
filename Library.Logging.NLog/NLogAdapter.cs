using Library.Logging.Abstractions;
using NLog;

namespace Library.Logging.NLog;

public class NLogAdapter : ILoggerPort
{
    private static readonly object _configLock = new();
    private static bool _configured;

    private readonly Logger _logger;

    public NLogAdapter(string loggerName)
    {
        EnsureConfigured();
        _logger = LogManager.GetLogger(loggerName);
    }

    private static void EnsureConfigured()
    {
        lock (_configLock)
        {
            if (_configured) return;
            var configFilePath = Path.Combine(AppContext.BaseDirectory, "NLog.config");
            if (!File.Exists(configFilePath))
                throw new FileNotFoundException($"NLog config file not found at: {configFilePath}");
            LogManager.Setup().LoadConfigurationFromFile(configFilePath, optional: false);
            _configured = true;
        }
    }

    public void Debug(string message) { _logger.Debug(message); }
    public void Info(string message) { _logger.Info(message); }
    public void Warn(string message) { _logger.Warn(message); }

    public void Error(string message, Exception? ex = null)
    {
        if (ex is not null)
            _logger.Error(ex, message);
        else
            _logger.Error(message);
    }
}