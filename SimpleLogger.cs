using SimpleLogger;
using System;
using System.IO;


namespace SerialJsonFilesSender
{
    public class SimpleLogger
    {
        private bool _isLogTrace;
        private bool _isLogDebug;
        private bool _isLogInfo;
        private bool _isLogWarn;
        private bool _isLogError;
        private bool _isLogFatal;
        private readonly LogLevel _logLevel;

        public SimpleLogger(LogLevel logLevel = LogLevel.Information)
        {
            //_logFilePath = logFilePath;
            _logLevel = logLevel;
            ConfigLogLevel();
        }
        private void ConfigLogLevel()
        {
            _isLogTrace = _isLogDebug = _isLogInfo = _isLogWarn = _isLogError = _isLogFatal = true;
            if(_logLevel == LogLevel.Debug) _isLogTrace = false;
            else if(_logLevel == LogLevel.Information) _isLogTrace = _isLogDebug = false;
            else if(_logLevel == LogLevel.Warning) _isLogTrace = _isLogDebug = _isLogInfo = false;
            else if(_logLevel == LogLevel.Error) _isLogTrace = _isLogDebug = _isLogInfo = _isLogWarn = false;
            else if(_logLevel == LogLevel.Critical) _isLogTrace = _isLogDebug = _isLogInfo = _isLogWarn = _isLogError = false;
            else if(_logLevel == LogLevel.None) _isLogTrace = _isLogDebug = _isLogInfo = _isLogWarn = _isLogError = _isLogFatal = false;
        }

        public void Log(string message)
        {
            var logMessage = $"{DateTime.Now}: {message}";
            Console.WriteLine(logMessage);
            string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"log_{DateTime.Now.ToString("ddMMyy")}.log");
            File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
        }

        public void LogTrace(string message)
        {
             if(_isLogTrace) Log($"TRACE: {message}");
        }
        public void LogError(string message)
        {
            if (_isLogError) Log($"ERROR: {message}");
        }
        public void LogWarn(string message)
        {
            if (_isLogWarn) Log($"WARNING: {message}");
        }
        public void LogDebug(string message)
        {
            if (_isLogDebug) Log($"DEBUG: {message}");
        }
        public void LogInfo(string message)
        {
            if (_isLogInfo) Log($"INFO: {message}");
        }
        public void LogFatal(string message)
        {
            if (_isLogFatal) Log($"FATAL: {message}");
        }
    }
}
