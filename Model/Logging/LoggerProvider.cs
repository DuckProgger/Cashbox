using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Cashbox.Model.Logging
{
    public class LoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new DB_Logger();
        }

        public void Dispose()
        {
        }

        private class DB_Logger : ILogger
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId,
                    TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                File.AppendAllText("dblog.txt", formatter(state, exception));
            }
        }
    }
}