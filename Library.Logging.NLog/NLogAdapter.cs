using Library.Logging.Abstractions;
using NLog;

namespace Library.Logging.NLog;

public class NLogAdapter : ILoggerPort
{
    private readonly Logger _logger;

    public NLogAdapter(string loggerName)
    {
        // AppContext.BaseDirectory punta alla cartella di esecuzione del servizio
        var configFilePath = Path.Combine(AppContext.BaseDirectory, "NLog.config");

        // Se vuoi essere sicuro che il file esista
        if (!File.Exists(configFilePath))
            throw new FileNotFoundException($"NLog config file not found at: {configFilePath}");

        // Carica la configurazione
        LogManager.Setup().LoadConfigurationFromFile(configFilePath, optional: false);

        _logger = LogManager.GetLogger(loggerName);
    }

    public void Debug(string message)
    {
        _logger.Debug(message);
    }

    public void Info(string message)
    {
        _logger.Info(message);
    }

    public void Warn(string message)
    {
        _logger.Warn(message);
    }

    public void Error(string message, Exception? ex = null)
    {
        if (ex is not null)
            _logger.Error(ex, message);
        else
            _logger.Error(message);
    }
}
