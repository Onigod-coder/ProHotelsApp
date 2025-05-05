using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HotelsApp.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Data.Entity;
using System.Windows.Media.Animation;

namespace HotelsApp
{
    public class AmenityViewModel : INotifyPropertyChanged
    {
        private bool _isSelected;
        public Amenities Amenity { get; set; }
        public string AmenityName => Amenity.AmenityName;
        public int AmenityID => Amenity.AmenityID;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class StatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status == "Confirmed" ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RoomViewModel : INotifyPropertyChanged
    {
        private decimal _totalPrice;
        private int _totalNights;

        public Rooms Room { get; set; }
        public string RoomNumber => Room.RoomNumber;
        public string RoomType => Room.RoomTypes.TypeName;
        public int Capacity => Room.RoomTypes.Capacity;
        public decimal BasePrice => Room.RoomTypes.BasePrice;
        public string Description => Room.RoomTypes.Description ?? "Описание отсутствует";
        public string RoomImage => $"/Resources/Rooms/room__{Room.RoomTypeID}.jpg";
        public decimal TotalPrice
        {
            get => _totalPrice;
            set
            {
                if (_totalPrice != value)
                {
                    _totalPrice = value;
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }
        public int TotalNights
        {
            get => _totalNights;
            set
            {
                if (_totalNights != value)
                {
                    _totalNights = value;
                    TotalPrice = BasePrice * value;
                    OnPropertyChanged(nameof(TotalNights));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BookingViewModel : INotifyPropertyChanged
    {
        public Bookings Booking { get; set; }
        public int BookingID => Booking.BookingID;
        public string HotelName => Booking.Rooms.Hotels.HotelName;
        public string RoomNumber => Booking.Rooms.RoomNumber;
        public string RoomType => Booking.Rooms.RoomTypes.TypeName;
        public DateTime CheckInDate => Booking.CheckInDate;
        public DateTime CheckOutDate => Booking.CheckOutDate;
        public decimal TotalPrice => Booking.TotalPrice;
        public string Status => Booking.Status;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class MainWindow : Window
    {
        private ProHotelEntities dbContext;
        private Customers currentUser;
        private List<AmenityViewModel> amenityViewModels;

        public MainWindow()
        {
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            try
            {
                dbContext = new ProHotelEntities();
                LoadInitialData();
                UpdateUserInterface();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации приложения: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void LoadInitialData()
        {
            try
            {
                // Загрузка отелей
                var hotels = dbContext.Hotels.ToList();
                foreach (var hotel in hotels)
                {
                    hotel.HotelImage = $"/Resources/Hotels/hotel__{hotel.HotelID}.jpg";
                }
                HotelsListView.ItemsSource = hotels;

                // Загрузка типов комнат
                var roomTypes = dbContext.RoomTypes.ToList();
                RoomTypeComboBox.ItemsSource = roomTypes;
                RoomTypeComboBox.DisplayMemberPath = "TypeName";
                RoomTypeComboBox.SelectedIndex = 0;

                // Загрузка городов
                var cities = dbContext.Addresses.Select(a => a.Cities).Distinct().ToList();
                CityComboBox.ItemsSource = cities;
                CityComboBox.SelectedIndex = 0;
                CityComboBox.SelectionChanged += CityComboBox_SelectionChanged;

                // Загрузка количества гостей
                GuestsComboBox.Items.Clear();
                GuestsComboBox.Items.Add(new ComboBoxItem { Content = "1 гость" });
                GuestsComboBox.Items.Add(new ComboBoxItem { Content = "2 гостя" });
                GuestsComboBox.Items.Add(new ComboBoxItem { Content = "3 гостя" });
                GuestsComboBox.Items.Add(new ComboBoxItem { Content = "4 гостя" });
                GuestsComboBox.Items.Add(new ComboBoxItem { Content = "5+ гостей" });
                GuestsComboBox.SelectedIndex = 0;

                // Загрузка дат
                CheckInDatePicker.SelectedDate = DateTime.Today;
                CheckOutDatePicker.SelectedDate = DateTime.Today.AddDays(1);

                var amenities = dbContext.Amenities.ToList();
                amenityViewModels = amenities.Select(a => new AmenityViewModel { Amenity = a }).ToList();
                AmenitiesListBox.ItemsSource = amenityViewModels;

                // Инициализация значений фильтров
                StarRatingComboBox.SelectedIndex = 0;
                MinPriceTextBox.Text = "0";
                MaxPriceTextBox.Text = "10000";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateUserInterface()
        {
            if (currentUser != null)
            {
                LoginButton.Content = "Выйти";
                RegisterButton.Visibility = Visibility.Collapsed;
                MyBookingsTab.Visibility = Visibility.Visible;
                UpdateProfileInfo(currentUser);
                LoadUserBookings();
                UserGreetingText.Text = $"Здравствуйте, {currentUser.FirstName} {currentUser.LastName}!";
            }
            else
            {
                LoginButton.Content = "Войти";
                RegisterButton.Visibility = Visibility.Visible;
                MyBookingsTab.Visibility = Visibility.Collapsed;
                ClearProfileInfo();
                UserGreetingText.Text = "Добро пожаловать в HotelsApp!";
            }
        }

        private void UpdateProfileInfo(Customers customer)
        {
            if (customer == null) return;

            ProfileFirstName.Text = customer.FirstName;
            ProfileLastName.Text = customer.LastName;
            ProfileEmail.Text = customer.Email;

            // Corrected the usage of ToString with a format specifier
            ProfileRegDate.Text = customer.RegistrationDate?.ToString("dd.MM.yyyy") ?? string.Empty;
        }

        private void ClearProfileInfo()
        {
            ProfileFirstName.Text = string.Empty;
            ProfileLastName.Text = string.Empty;
            ProfileEmail.Text = string.Empty;
            ProfileRegDate.Text = string.Empty;
            BookingsListView.ItemsSource = null;
        }

        private void LoadUserBookings()
        {
            if (currentUser == null) return;

            try
            {
                var bookings = dbContext.Bookings
                    .Include(b => b.Rooms.Hotels)
                    .Include(b => b.Rooms.RoomTypes)
                    .Where(b => b.CustomerID == currentUser.CustomerID)
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();

                var bookingViewModels = bookings.Select(b => new BookingViewModel { Booking = b }).ToList();
                BookingsListView.ItemsSource = bookingViewModels;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке бронирований: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedCity = (Cities)CityComboBox.SelectedItem;
                var checkInDate = CheckInDatePicker.SelectedDate;
                var checkOutDate = CheckOutDatePicker.SelectedDate;
                var selectedRoomType = (RoomTypes)RoomTypeComboBox.SelectedItem;
                var selectedGuests = GuestsComboBox.SelectedItem as ComboBoxItem;

                if (selectedCity == null)
                {
                    MessageBox.Show("Пожалуйста, выберите город", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (checkInDate == null || checkOutDate == null)
                {
                    MessageBox.Show("Пожалуйста, выберите даты заезда и выезда", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (checkInDate >= checkOutDate)
                {
                    MessageBox.Show("Дата выезда должна быть позже даты заезда", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var hotels = dbContext.Hotels
                    .Where(h => h.Addresses.CityID == selectedCity.CityID);

                var selectedStarRating = StarRatingComboBox.SelectedIndex;
                if (selectedStarRating > 0)
                {
                    hotels = hotels.Where(h => h.StarRating == selectedStarRating);
                }

                if (selectedRoomType != null)
                {
                    hotels = hotels.Where(h => h.Rooms.Any(r => r.RoomTypeID == selectedRoomType.RoomTypeID && r.IsAvailable == true));
                }

                if (selectedGuests != null)
                {
                    int guestsCount = 1;
                    string guestsText = selectedGuests.Content.ToString();

                    if (guestsText.Contains("5+"))
                    {
                        guestsCount = 5;
                    }
                    else
                    {
                        int.TryParse(guestsText.Split(' ')[0], out guestsCount);
                    }

                    hotels = hotels.Where(h => h.Rooms.Any(r =>
                        r.RoomTypes.Capacity >= guestsCount && r.IsAvailable == true));
                }

                var selectedAmenities = amenityViewModels
                    .Where(a => a.IsSelected)
                    .Select(a => a.AmenityID)
                    .ToList();

                if (selectedAmenities.Any())
                {
                    hotels = hotels.Where(h => h.Rooms.Any(r =>
                        r.Amenities.Any(a => selectedAmenities.Contains(a.AmenityID))));
                }

                var filteredHotels = hotels.ToList();
                HotelsListView.ItemsSource = filteredHotels;

                if (filteredHotels.Any())
                {
                    HotelsListView.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Отели не найдены по заданным критериям", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске отелей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HotelsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedHotel = HotelsListView.SelectedItem as Hotels;
            if (selectedHotel != null)
            {
                var selectedRoomType = RoomTypeComboBox.SelectedItem as RoomTypes;
                var selectedGuestItem = GuestsComboBox.SelectedItem as ComboBoxItem;
                int selectedGuestCount = 1;
                
                if (selectedGuestItem != null)
                {
                    string guestText = selectedGuestItem.Content.ToString();
                    if (guestText.Contains("5+"))
                    {
                        selectedGuestCount = 5;
                    }
                    else
                    {
                        int.TryParse(guestText.Split(' ')[0], out selectedGuestCount);
                    }
                }

                var checkInDate = CheckInDatePicker.SelectedDate ?? DateTime.Today;
                var checkOutDate = CheckOutDatePicker.SelectedDate ?? DateTime.Today.AddDays(1);

                var availableRooms = dbContext.Rooms
                    .Where(r => r.HotelID == selectedHotel.HotelID)
                    .Where(r => r.RoomTypes.Capacity >= selectedGuestCount);

                if (selectedRoomType != null)
                {
                    availableRooms = availableRooms.Where(r => r.RoomTypeID == selectedRoomType.RoomTypeID);
                }

                availableRooms = availableRooms.Where(r => !dbContext.Bookings.Any(b => 
                    b.RoomID == r.RoomID && 
                    b.Status == "Confirmed" && 
                    ((b.CheckInDate <= checkInDate && b.CheckOutDate > checkInDate) ||
                     (b.CheckInDate < checkOutDate && b.CheckOutDate >= checkOutDate) ||
                     (b.CheckInDate >= checkInDate && b.CheckOutDate <= checkOutDate))));

                var roomsList = availableRooms.ToList();
                var roomViewModels = roomsList.Select(r => new RoomViewModel
                {
                    Room = r,
                    TotalNights = (int)(checkOutDate - checkInDate).TotalDays
                }).ToList();

                RoomsListView.ItemsSource = roomViewModels;
                AmenitiesListView.ItemsSource = dbContext.Amenities
                    .Where(a => a.Rooms.Any(r => r.HotelID == selectedHotel.HotelID))
                    .Distinct()
                    .ToList();

                ReviewsListView.ItemsSource = dbContext.Reviews
                    .Where(r => r.Bookings.Rooms.HotelID == selectedHotel.HotelID)
                    .OrderByDescending(r => r.ReviewDate)
                    .ToList();
            }
            else
            {
                RoomsListView.ItemsSource = null;
            }
        }

        private void BookRoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser == null)
            {
                MessageBox.Show("Для бронирования номера необходимо войти в систему", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var button = (Button)sender;
            var roomViewModel = (RoomViewModel)button.DataContext;
            var selectedRoom = roomViewModel.Room;
            var selectedHotel = (Hotels)HotelsListView.SelectedItem;
            var checkInDate = CheckInDatePicker.SelectedDate;
            var checkOutDate = CheckOutDatePicker.SelectedDate;

            if (selectedRoom == null || selectedHotel == null || checkInDate == null || checkOutDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите номер и укажите даты заезда и выезда", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (MessageBox.Show($"Вы уверены, что хотите забронировать номер?\n\n" +
                                  $"Отель: {selectedHotel.HotelName}\n" +
                                  $"Номер: {selectedRoom.RoomNumber}\n" +
                                  $"Тип номера: {selectedRoom.RoomTypes.TypeName}\n" +
                                  $"Даты: {checkInDate.Value:d} - {checkOutDate.Value:d}\n" +
                                  $"Количество ночей: {roomViewModel.TotalNights}\n" +
                                  $"Цена за ночь: ${selectedRoom.RoomTypes.BasePrice:N2}\n" +
                                  $"Итого к оплате: ${roomViewModel.TotalPrice:N2}",
                                  "Подтверждение бронирования",
                                  MessageBoxButton.YesNo,
                                  MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var booking = new Bookings
                    {
                        CustomerID = currentUser.CustomerID,
                        RoomID = selectedRoom.RoomID,
                        BookingDate = DateTime.Now,
                        CheckInDate = checkInDate.Value,
                        CheckOutDate = checkOutDate.Value,
                        TotalPrice = roomViewModel.TotalPrice,
                        Status = "Confirmed"
                    };

                    dbContext.Bookings.Add(booking);
                    dbContext.SaveChanges();

                    MessageBox.Show($"Номер успешно забронирован! Сумма к оплате: ${roomViewModel.TotalPrice:N2}",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadUserBookings();
                    UpdateProfileInfo(currentUser);
                    HotelsListView_SelectionChanged(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при бронировании номера: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBookingButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var bookingViewModel = (BookingViewModel)button.DataContext;
            var booking = bookingViewModel.Booking;

            if (booking == null) return;

            try
            {
                if (booking.Status == "Cancelled")
                {
                    MessageBox.Show("Это бронирование уже отменено", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (MessageBox.Show("Вы уверены, что хотите отменить это бронирование?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    booking.Status = "Cancelled";
                    dbContext.SaveChanges();
                    LoadUserBookings();
                    UpdateProfileInfo(currentUser);
                    HotelsListView_SelectionChanged(null, null);
                    MessageBox.Show("Бронирование успешно отменено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отмене бронирования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowAuthNotification()
        {
            AuthNotification.Visibility = Visibility.Visible;
            AuthNotification.Opacity = 0;
            var animation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            AuthNotification.BeginAnimation(UIElement.OpacityProperty, animation);

            // Скрываем уведомление через 2.5 секунды
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2.5);
            timer.Tick += (s, e) =>
            {
                var fadeOut = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.5)
                };
                fadeOut.Completed += (sender, args) =>
                {
                    AuthNotification.Visibility = Visibility.Collapsed;
                };
                AuthNotification.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                timer.Stop();
            };
            timer.Start();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser != null)
            {
                currentUser = null;
                UpdateUserInterface();
                ClearProfileInfo();
                BookingsListView.ItemsSource = null;
                MessageBox.Show("Вы успешно вышли из системы", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var loginWindow = new LoginWindow(dbContext);
                if (loginWindow.ShowDialog() == true)
                {
                    currentUser = loginWindow.AuthenticatedCustomer;
                    UpdateUserInterface();
                    ShowAuthNotification();
                }
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow(dbContext);
            if (registerWindow.ShowDialog() == true)
            {
                currentUser = registerWindow.RegisteredCustomer;
                UpdateUserInterface();
                ShowAuthNotification();
            }
        }

        private void ViewAllHotelsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (HotelsListView == null)
                {
                    MessageBox.Show("HotelsListView is not initialized.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                HotelsListView.ItemsSource = dbContext.Hotels.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отелей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddReviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser == null)
            {
                MessageBox.Show("Для оставления отзыва необходимо войти в систему", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedHotel = (Hotels)HotelsListView.SelectedItem;
            if (selectedHotel == null)
            {
                MessageBox.Show("Пожалуйста, выберите отель", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var reviewWindow = new AddReviewWindow(dbContext, selectedHotel.HotelID, currentUser.CustomerID);
            if (reviewWindow.ShowDialog() == true)
            {
                ReviewsListView.ItemsSource = dbContext.Reviews
                    .Where(r => r.Bookings.Rooms.HotelID == selectedHotel.HotelID)
                    .OrderByDescending(r => r.ReviewDate)
                    .ToList();
            }
        }

        private void RoomTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HotelsListView.SelectedItem != null)
            {
                HotelsListView_SelectionChanged(null, null);
            }
        }

        private void GuestsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HotelsListView?.SelectedItem != null)
            {
                HotelsListView_SelectionChanged(null, null);
            }
        }

        private void PayBookingButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var bookingViewModel = (BookingViewModel)button.DataContext;
            var booking = bookingViewModel.Booking;

            if (booking == null) return;

            try
            {
                if (booking.Status == "Paid")
                {
                    MessageBox.Show("Это бронирование уже оплачено", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (booking.Status == "Cancelled")
                {
                    MessageBox.Show("Это бронирование отменено и не может быть оплачено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (MessageBox.Show($"Вы уверены, что хотите оплатить бронирование?\n\n" +
                                  $"Отель: {booking.Rooms.Hotels.HotelName}\n" +
                                  $"Номер: {booking.Rooms.RoomNumber}\n" +
                                  $"Даты: {booking.CheckInDate:d} - {booking.CheckOutDate:d}\n" +
                                  $"Сумма к оплате: {booking.TotalPrice:C}",
                                  "Подтверждение оплаты",
                                  MessageBoxButton.YesNo,
                                  MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    booking.Status = "Paid";
                    dbContext.SaveChanges();
                    MessageBox.Show("Оплата успешна. Спасибо за то, что воспользовались нашим приложением. Приятного отдыха!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    UpdateProfileInfo(currentUser);
                    LoadUserBookings();
                    HotelsListView_SelectionChanged(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оплате бронирования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HotelsListView?.SelectedItem != null)
            {
                var checkInDate = CheckInDatePicker.SelectedDate ?? DateTime.Today;
                var checkOutDate = CheckOutDatePicker.SelectedDate ?? DateTime.Today.AddDays(1);

                if (checkInDate >= checkOutDate)
                {
                    return;
                }

                var rooms = RoomsListView.ItemsSource as List<RoomViewModel>;
                if (rooms != null)
                {
                    foreach (var room in rooms)
                    {
                        room.TotalNights = (int)(checkOutDate - checkInDate).TotalDays;
                    }
                }
            }
        }

        private void CityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HotelsListView?.SelectedItem != null)
            {
                HotelsListView_SelectionChanged(null, null);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            dbContext?.Dispose();
        }
    }
}