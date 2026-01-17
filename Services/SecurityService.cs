using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using HotelsApp.Model;

namespace HotelsApp.Services
{
    public class SecurityService
    {
        private readonly string _salt;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;

        public SecurityService()
        {
            _salt = ConfigurationManager.AppSettings["PasswordSalt"] ?? "defaultSalt";
            _smtpServer = ConfigurationManager.AppSettings["SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
            _smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"] ?? "";
            _smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"] ?? "";
            _fromEmail = ConfigurationManager.AppSettings["FromEmail"] ?? "";
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + _salt;
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }

        public string GenerateTwoFactorCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public void SendTwoFactorCode(string email, string code)
        {
            try
            {
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    client.EnableSsl = true;

                    var message = new MailMessage(_fromEmail, email)
                    {
                        Subject = "Код двухфакторной аутентификации",
                        Body = $"Ваш код подтверждения: {code}\n\nКод действителен в течение 5 минут.",
                        IsBodyHtml = false
                    };

                    client.Send(message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при отправке кода подтверждения", ex);
            }
        }

        public bool ValidateTwoFactorCode(string storedCode, string enteredCode, DateTime codeGenerationTime)
        {
            if (string.IsNullOrEmpty(storedCode) || string.IsNullOrEmpty(enteredCode))
                return false;

            // Проверяем, что код не старше 5 минут
            if ((DateTime.Now - codeGenerationTime).TotalMinutes > 5)
                return false;

            return storedCode == enteredCode;
        }

        public string GeneratePasswordResetToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);
            }
        }

        public void SendPasswordResetLink(string email, string resetToken)
        {
            try
            {
                var resetLink = $"{ConfigurationManager.AppSettings["ResetPasswordUrl"]}?token={resetToken}";

                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    client.EnableSsl = true;

                    var message = new MailMessage(_fromEmail, email)
                    {
                        Subject = "Сброс пароля",
                        Body = $"Для сброса пароля перейдите по ссылке: {resetLink}\n\nСсылка действительна в течение 1 часа.",
                        IsBodyHtml = false
                    };

                    client.Send(message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при отправке ссылки для сброса пароля", ex);
            }
        }

        public bool ValidatePasswordResetToken(string storedToken, string providedToken, DateTime tokenGenerationTime)
        {
            if (string.IsNullOrEmpty(storedToken) || string.IsNullOrEmpty(providedToken))
                return false;

            // Проверяем, что токен не старше 1 часа
            if ((DateTime.Now - tokenGenerationTime).TotalHours > 1)
                return false;

            return storedToken == providedToken;
        }

        public bool IsPasswordStrong(string password)
        {
            // Минимум 8 символов
            if (password.Length < 8)
                return false;

            // Содержит хотя бы одну цифру
            if (!password.Any(char.IsDigit))
                return false;

            // Содержит хотя бы одну заглавную букву
            if (!password.Any(char.IsUpper))
                return false;

            // Содержит хотя бы одну строчную букву
            if (!password.Any(char.IsLower))
                return false;

            // Содержит хотя бы один специальный символ
            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                return false;

            return true;
        }
    }
} 