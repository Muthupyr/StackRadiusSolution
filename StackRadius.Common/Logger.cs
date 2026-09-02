using System;
using System.Collections;
using log4net;

namespace StackRadius.Common
{
    public static class Logger
    {
        static ICollection _log4netConfiguration = null;
        static ILog _logger = null;

        private static void CheckLogger()
        {
            if (_log4netConfiguration == null)
            {
                _log4netConfiguration = log4net.Config.XmlConfigurator.Configure();
                _logger = LogManager.GetLogger("Logger");
            }
        }

        public static void LogError(Exception ex)
        {
            CheckLogger();
            _logger.Error(ex);
        }

        public static void LogDebug(string msg)
        {
            CheckLogger();
            _logger.Debug(msg?.Replace("\n", string.Empty));
        }
    }
}
