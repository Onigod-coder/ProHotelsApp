using System;
using System.IO;
using Xunit;
using HotelsApp.Services;

namespace HotelsApp.Tests
{
    public class LoggingServiceTests : IDisposable
    {
        private readonly string _testLogDirectory;
        private readonly LoggingService _loggingService;

        public LoggingServiceTests()
        {
            _testLogDirectory = Path.Combine(Path.GetTempPath(), "HotelsAppTestLogs");
            _loggingService = new LoggingService();
        }

        [Fact]
        public void LogError_WithException_LogsError()
        {
            // Arrange
            var exception = new Exception("Test error");
            var context = "Test context";

            // Act
            _loggingService.LogError(exception, context);

            // Assert
            var logFile = Path.Combine(_testLogDirectory, $"hotelsapp_{DateTime.Now:yyyyMMdd}.log");
            Assert.True(File.Exists(logFile));
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("ERROR", logContent);
            Assert.Contains("Test error", logContent);
            Assert.Contains("Test context", logContent);
        }

        [Fact]
        public void LogInfo_WithMessage_LogsInfo()
        {
            // Arrange
            var message = "Test info message";

            // Act
            _loggingService.LogInfo(message);

            // Assert
            var logFile = Path.Combine(_testLogDirectory, $"hotelsapp_{DateTime.Now:yyyyMMdd}.log");
            Assert.True(File.Exists(logFile));
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("INFO", logContent);
            Assert.Contains(message, logContent);
        }

        [Fact]
        public void LogWarning_WithMessage_LogsWarning()
        {
            // Arrange
            var message = "Test warning message";

            // Act
            _loggingService.LogWarning(message);

            // Assert
            var logFile = Path.Combine(_testLogDirectory, $"hotelsapp_{DateTime.Now:yyyyMMdd}.log");
            Assert.True(File.Exists(logFile));
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("WARNING", logContent);
            Assert.Contains(message, logContent);
        }

        [Fact]
        public void GetLogContent_WithExistingLog_ReturnsLogContent()
        {
            // Arrange
            var date = DateTime.Now;
            var expectedContent = "Test log content";
            var logFile = Path.Combine(_testLogDirectory, $"hotelsapp_{date:yyyyMMdd}.log");
            File.WriteAllText(logFile, expectedContent);

            // Act
            var content = _loggingService.GetLogContent(date);

            // Assert
            Assert.Equal(expectedContent, content);
        }

        [Fact]
        public void GetLogContent_WithNonExistentLog_ReturnsNotFoundMessage()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-1);

            // Act
            var content = _loggingService.GetLogContent(date);

            // Assert
            Assert.Equal("Лог-файл не найден", content);
        }

        [Fact]
        public void ClearOldLogs_WithOldLogs_RemovesOldLogs()
        {
            // Arrange
            var oldDate = DateTime.Now.AddDays(-10);
            var newDate = DateTime.Now;
            var oldLogFile = Path.Combine(_testLogDirectory, $"hotelsapp_{oldDate:yyyyMMdd}.log");
            var newLogFile = Path.Combine(_testLogDirectory, $"hotelsapp_{newDate:yyyyMMdd}.log");

            File.WriteAllText(oldLogFile, "Old log");
            File.WriteAllText(newLogFile, "New log");

            // Act
            _loggingService.ClearOldLogs(5); // Удаляем логи старше 5 дней

            // Assert
            Assert.False(File.Exists(oldLogFile));
            Assert.True(File.Exists(newLogFile));
        }

        public void Dispose()
        {
            // Очищаем тестовые файлы после каждого теста
            if (Directory.Exists(_testLogDirectory))
            {
                Directory.Delete(_testLogDirectory, true);
            }
        }
    }
} 