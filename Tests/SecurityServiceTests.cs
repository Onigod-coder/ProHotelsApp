using System;
using Xunit;
using HotelsApp.Services;

namespace HotelsApp.Tests
{
    public class SecurityServiceTests
    {
        private readonly SecurityService _securityService;

        public SecurityServiceTests()
        {
            _securityService = new SecurityService();
        }

        [Fact]
        public void HashPassword_WithSamePassword_ReturnsSameHash()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hash1 = _securityService.HashPassword(password);
            var hash2 = _securityService.HashPassword(password);

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void HashPassword_WithDifferentPasswords_ReturnsDifferentHashes()
        {
            // Arrange
            var password1 = "TestPassword123!";
            var password2 = "TestPassword124!";

            // Act
            var hash1 = _securityService.HashPassword(password1);
            var hash2 = _securityService.HashPassword(password2);

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "TestPassword123!";
            var hashedPassword = _securityService.HashPassword(password);

            // Act
            var result = _securityService.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var password = "TestPassword123!";
            var wrongPassword = "TestPassword124!";
            var hashedPassword = _securityService.HashPassword(password);

            // Act
            var result = _securityService.VerifyPassword(wrongPassword, hashedPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GenerateTwoFactorCode_ReturnsSixDigitNumber()
        {
            // Act
            var code = _securityService.GenerateTwoFactorCode();

            // Assert
            Assert.True(int.TryParse(code, out int number));
            Assert.True(number >= 100000 && number <= 999999);
        }

        [Fact]
        public void ValidateTwoFactorCode_WithValidCode_ReturnsTrue()
        {
            // Arrange
            var code = "123456";
            var generationTime = DateTime.Now;

            // Act
            var result = _securityService.ValidateTwoFactorCode(code, code, generationTime);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateTwoFactorCode_WithExpiredCode_ReturnsFalse()
        {
            // Arrange
            var code = "123456";
            var generationTime = DateTime.Now.AddMinutes(-6); // Код старше 5 минут

            // Act
            var result = _securityService.ValidateTwoFactorCode(code, code, generationTime);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GeneratePasswordResetToken_ReturnsValidToken()
        {
            // Act
            var token = _securityService.GeneratePasswordResetToken();

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            Assert.True(token.Length > 32); // Минимальная длина токена
        }

        [Fact]
        public void ValidatePasswordResetToken_WithValidToken_ReturnsTrue()
        {
            // Arrange
            var token = _securityService.GeneratePasswordResetToken();
            var generationTime = DateTime.Now;

            // Act
            var result = _securityService.ValidatePasswordResetToken(token, token, generationTime);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidatePasswordResetToken_WithExpiredToken_ReturnsFalse()
        {
            // Arrange
            var token = _securityService.GeneratePasswordResetToken();
            var generationTime = DateTime.Now.AddHours(-2); // Токен старше 1 часа

            // Act
            var result = _securityService.ValidatePasswordResetToken(token, token, generationTime);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("Password123!", true)] // Сильный пароль
        [InlineData("password", false)] // Нет цифр и заглавных букв
        [InlineData("PASSWORD123", false)] // Нет строчных букв
        [InlineData("Password", false)] // Нет цифр
        [InlineData("12345678", false)] // Нет букв
        [InlineData("Pass1!", true)] // Минимальные требования
        [InlineData("Pass1", false)] // Нет специального символа
        public void IsPasswordStrong_WithDifferentPasswords_ReturnsExpectedResult(string password, bool expected)
        {
            // Act
            var result = _securityService.IsPasswordStrong(password);

            // Assert
            Assert.Equal(expected, result);
        }
    }
} 