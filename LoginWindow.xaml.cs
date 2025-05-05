using HotelsApp.Model;
using System;
using System.Linq;
using System.Windows;

namespace HotelsApp
{
    public partial class LoginWindow : Window
    {   
        private ProHotelEntities dbContext;
        public Customers AuthenticatedCustomer { get; private set; }

        public LoginWindow(ProHotelEntities context)
        {
            InitializeComponent();
            dbContext = context;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите email и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var customer = dbContext.Customers.FirstOrDefault(c => c.Email == email);

                if (customer == null || customer.Password != password)
                {
                    MessageBox.Show("Неверный email или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                AuthenticatedCustomer = customer;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе в систему: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}