using Microsoft.Extensions.Logging;
using System;

namespace Stuble
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, Exception> _unhandledException = LoggerMessage.Define(LogLevel.Error, new EventId(0, "UnhandledException"), "An unhandled exception has occured");

        public static void LogUnhandledException(this ILogger logger, Exception ex) => _unhandledException(logger, ex);
    }
}
