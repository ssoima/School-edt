using System;
using N = NLog;

namespace NextLAP.IP1.Common.Diagnostics
{
    public class Logger
    {
        private readonly N.Logger _nlog;

        internal Logger(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            _nlog = N.LogManager.GetLogger(type.Name);
        }

        internal Logger(string name)
        {
            _nlog = N.LogManager.GetLogger(name);
        }

        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        public void Warning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        public void Fatal(string message)
        {
            Log(LogLevel.Fatal, message);
        }
        public void Log(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Info:
                    _nlog.Log(N.LogLevel.Info, message);
                    break;
                case LogLevel.Debug:
                    _nlog.Log(N.LogLevel.Debug, message);
                    break;
                case LogLevel.Warning:
                    _nlog.Log(N.LogLevel.Warn, message);
                    break;
                case LogLevel.Error:
                    _nlog.Log(N.LogLevel.Error, message);
                    break;
                case LogLevel.Fatal:
                    _nlog.Log(N.LogLevel.Fatal, message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("level");
            }
        }
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }
}
