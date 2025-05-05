using HotelsApp.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace HotelsApp
{
    public partial class AddReviewWindow : Window
    {
        private readonly ProHotelEntities _dbContext;
        private readonly int _hotelId;
        private readonly int _customerId;

        public AddReviewWindow(ProHotelEntities dbContext, int hotelId, int customerId)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _hotelId = hotelId;
            _customerId = customerId;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CommentTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, напишите комментарий", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Находим бронирование этого пользователя в этом отеле
                var booking = _dbContext.Bookings.FirstOrDefault(b => b.Rooms.HotelID == _hotelId && b.CustomerID == _customerId);

                if (booking == null)
                {
                    MessageBox.Show("Вы не бронировали номер в этом отеле", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var review = new Reviews
                {
                    BookingID = booking.BookingID,
                    Rating = int.Parse((string)((ComboBoxItem)RatingComboBox.SelectedItem).Content),
                    Comment = CommentTextBox.Text,
                    ReviewDate = DateTime.Now
                };

                _dbContext.Reviews.Add(review);
                _dbContext.SaveChanges();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении отзыва: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}