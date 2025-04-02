using System;
using System.IO;

namespace SerialJsonFilesSender
{
    public class SimpleLogger
    {
        private readonly string _logFilePath;

        public SimpleLogger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public void Log(string message)
        {
            var logMessage = $"{DateTime.Now}: {message}";
            Console.WriteLine(logMessage);

            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }

        public void LogError(string message)
        {
            Log($"ERROR: {message}");
        }
        public void LogWarn(string message)
        {
            Log($"WARNING: {message}");
        }
        public void LogInfo(string message)
        {
            Log($"INFO: {message}");
        }
    }
}
