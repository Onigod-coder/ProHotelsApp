using HotelsApp.Model;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HotelsApp
{
    public partial class RegisterWindow : Window
    {
        private ProHotelEntities dbContext;
        public Customers RegisteredCustomer { get; private set; }

        public RegisterWindow(ProHotelEntities context)
        {
            InitializeComponent();
            dbContext = context;
            PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
            ConfirmPasswordBox.PasswordChanged += ConfirmPasswordBox_PasswordChanged;
            SetPasswordRequirementsText();
        }

        private void SetPasswordRequirementsText()
        {
            PasswordRequirementsText.Text = "Пароль должен содержать:\n- минимум 8 символов\n- хотя бы 1 цифру\n- хотя бы 1 спецсимвол (!@#$%^&*)";
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateAllFields())
                return;

            try
            {
                string email = EmailTextBox.Text.Trim();

                if (dbContext.Customers.Any(c => c.Email == email))
                {
                    ShowErrorMessage("Пользователь с таким email уже зарегистрирован", EmailTextBox);
                    return;
                }

                var customer = new Customers
                {
                    FirstName = FirstNameTextBox.Text.Trim(),
                    LastName = LastNameTextBox.Text.Trim(),
                    Email = email,
                    Password = PasswordBox.Password,
                    RegistrationDate = DateTime.Now
                };

                dbContext.Customers.Add(customer);
                dbContext.SaveChanges();

                RegisteredCustomer = customer;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateAllFields()
        {
            // Проверка имени
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                ShowErrorMessage("Введите имя", FirstNameTextBox);
                return false;
            }

            if (!IsValidName(FirstNameTextBox.Text))
            {
                ShowErrorMessage("Имя содержит недопустимые символы", FirstNameTextBox);
                return false;
            }

            // Проверка фамилии
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                ShowErrorMessage("Введите фамилию", LastNameTextBox);
                return false;
            }

            if (!IsValidName(LastNameTextBox.Text))
            {
                ShowErrorMessage("Фамилия содержит недопустимые символы", LastNameTextBox);
                return false;
            }

            // Проверка email
            string email = EmailTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowErrorMessage("Введите email", EmailTextBox);
                return false;
            }

            if (!IsValidEmail(email))
            {
                ShowErrorMessage("Введите корректный email адрес", EmailTextBox);
                return false;
            }

            // Проверка пароля
            string password = PasswordBox.Password;
            if (string.IsNullOrWhiteSpace(password))
            {
                ShowErrorMessage("Введите пароль", PasswordBox);
                return false;
            }

            if (!IsPasswordStrong(password))
            {
                ShowErrorMessage("Пароль не соответствует требованиям", PasswordBox);
                return false;
            }

            // Проверка подтверждения пароля
            if (password != ConfirmPasswordBox.Password)
            {
                ShowErrorMessage("Пароли не совпадают", ConfirmPasswordBox);
                return false;
            }

            return true;
        }

        private bool IsValidName(string name)
        {
            // Разрешаем буквы, дефисы и апострофы
            return Regex.IsMatch(name, @"^[\p{L}\-' ]+$");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private bool IsPasswordStrong(string password)
        {
            // Минимум 8 символов, хотя бы 1 цифра и 1 спецсимвол
            return password.Length >= 8 &&
                   Regex.IsMatch(password, @"\d") &&
                   Regex.IsMatch(password, @"[!@#$%^&*]");
        }

        private void ShowErrorMessage(string message, Control control)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            control.Focus();
            if (control is TextBox textBox)
                textBox.SelectAll();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdatePasswordStrengthIndicator();
            CheckPasswordsMatch();
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckPasswordsMatch();
        }

        private void UpdatePasswordStrengthIndicator()
        {
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(password))
            {
                PasswordStrengthText.Text = "";
                PasswordStrengthBar.Value = 0;
                return;
            }

            int strength = 0;

            // Длина пароля
            if (password.Length >= 8) strength++;
            if (password.Length >= 12) strength++;

            // Сложность
            if (Regex.IsMatch(password, @"\d")) strength++;          // цифры
            if (Regex.IsMatch(password, @"[a-z]")) strength++;      // строчные буквы
            if (Regex.IsMatch(password, @"[A-Z]")) strength++;      // заглавные буквы
            if (Regex.IsMatch(password, @"[!@#$%^&*]")) strength++; // спецсимволы

            PasswordStrengthBar.Value = strength * 20; // 0-100% (5 критериев)

            // Замена switch expression на классический switch
            switch (strength)
            {
                case 0:
                case 1:
                    PasswordStrengthText.Text = "Очень слабый";
                    PasswordStrengthText.Foreground = Brushes.Red;
                    break;
                case 2:
                    PasswordStrengthText.Text = "Слабый";
                    PasswordStrengthText.Foreground = Brushes.Orange;
                    break;
                case 3:
                    PasswordStrengthText.Text = "Средний";
                    PasswordStrengthText.Foreground = Brushes.Yellow;
                    break;
                case 4:
                    PasswordStrengthText.Text = "Сильный";
                    PasswordStrengthText.Foreground = Brushes.LightGreen;
                    break;
                case 5:
                    PasswordStrengthText.Text = "Очень сильный";
                    PasswordStrengthText.Foreground = Brushes.Green;
                    break;
                default:
                    PasswordStrengthText.Text = "";
                    PasswordStrengthText.Foreground = Brushes.Black;
                    break;
            }
        }

        private void CheckPasswordsMatch()
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                PasswordMatchText.Text = "";
                return;
            }

            if (PasswordBox.Password == ConfirmPasswordBox.Password)
            {
                PasswordMatchText.Text = "✓ Пароли совпадают";
                PasswordMatchText.Foreground = Brushes.Green;
            }
            else if (!string.IsNullOrEmpty(ConfirmPasswordBox.Password))
            {
                PasswordMatchText.Text = "✗ Пароли не совпадают";
                PasswordMatchText.Foreground = Brushes.Red;
            }
            else
            {
                PasswordMatchText.Text = "";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}