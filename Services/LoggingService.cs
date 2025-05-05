using System;
using System.IO;
using System.Configuration;

namespace HotelsApp.Services
{
    public class LoggingService
    {
        private readonly string _logDirectory;
        private readonly string _logFileName;

        public LoggingService()
        {
            _logDirectory = ConfigurationManager.AppSettings["LogDirectory"] ?? "Logs";
            _logFileName = $"hotelsapp_{DateTime.Now:yyyyMMdd}.log";

            // Создаем директорию для логов, если она не существует
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public void LogError(Exception ex, string context = "")
        {
            try
            {
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR - {context}\n" +
                               $"Exception: {ex.GetType().Name}\n" +
                               $"Message: {ex.Message}\n" +
                               $"Stack Trace: {ex.StackTrace}\n" +
                               "----------------------------------------\n";

                WriteToLog(logMessage);
            }
            catch
            {
                // Если произошла ошибка при логировании, игнорируем её
            }
        }

        public void LogInfo(string message)
        {
            try
            {
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO - {message}\n";
                WriteToLog(logMessage);
            }
            catch
            {
                // Если произошла ошибка при логировании, игнорируем её
            }
        }

        public void LogWarning(string message)
        {
            try
            {
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] WARNING - {message}\n";
                WriteToLog(logMessage);
            }
            catch
            {
                // Если произошла ошибка при логировании, игнорируем её
            }
        }

        private void WriteToLog(string message)
        {
            var logPath = Path.Combine(_logDirectory, _logFileName);
            File.AppendAllText(logPath, message);
        }

        public string GetLogContent(DateTime date)
        {
            try
            {
                var logFileName = $"hotelsapp_{date:yyyyMMdd}.log";
                var logPath = Path.Combine(_logDirectory, logFileName);

                if (File.Exists(logPath))
                {
                    return File.ReadAllText(logPath);
                }

                return "Лог-файл не найден";
            }
            catch (Exception ex)
            {
                return $"Ошибка при чтении лог-файла: {ex.Message}";
            }
        }

        public void ClearOldLogs(int daysToKeep)
        {
            try
            {
                var files = Directory.GetFiles(_logDirectory, "hotelsapp_*.log");
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

                foreach (var file in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileName.Length >= 8)
                    {
                        var dateStr = fileName.Substring(fileName.Length - 8);
                        if (DateTime.TryParseExact(dateStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var fileDate))
                        {
                            if (fileDate < cutoffDate)
                            {
                                File.Delete(file);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Если произошла ошибка при удалении старых логов, игнорируем её
            }
        }
    }
} 