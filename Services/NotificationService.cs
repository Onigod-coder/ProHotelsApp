using System;
using System.Net.Mail;
using HotelsApp.Model;

namespace HotelsApp.Services
{
    public class NotificationService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;

        public NotificationService(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, string fromEmail)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
            _fromEmail = fromEmail;
        }

        public void SendBookingConfirmation(Bookings booking)
        {
            try
            {
                var customer = booking.Customers;
                var hotel = booking.Rooms.Hotels;

                var subject = "Подтверждение бронирования";
                var body = $@"
Уважаемый(ая) {customer.FirstName} {customer.LastName},

Ваше бронирование успешно подтверждено!

Детали бронирования:
Отель: {hotel.HotelName}
Номер: {booking.Rooms.RoomNumber}
Дата заезда: {booking.CheckInDate.ToShortDateString()}
Дата выезда: {booking.CheckOutDate.ToShortDateString()}
Стоимость: {booking.TotalPrice:C}

Спасибо за выбор нашего сервиса!
";

                SendEmail(customer.Email, subject, body);
            }
            catch (Exception ex)
            {
                // Логирование ошибки будет реализовано позже
                throw new Exception("Ошибка при отправке подтверждения бронирования", ex);
            }
        }

        public void SendBookingCancellation(Bookings booking)
        {
            try
            {
                var customer = booking.Customers;
                var hotel = booking.Rooms.Hotels;

                var subject = "Отмена бронирования";
                var body = $@"
Уважаемый(ая) {customer.FirstName} {customer.LastName},

Ваше бронирование в отеле {hotel.HotelName} отменено.

Детали отмененного бронирования:
Номер: {booking.Rooms.RoomNumber}
Дата заезда: {booking.CheckInDate.ToShortDateString()}
Дата выезда: {booking.CheckOutDate.ToShortDateString()}

Если у вас есть вопросы, пожалуйста, свяжитесь с нами.
";

                SendEmail(customer.Email, subject, body);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при отправке уведомления об отмене бронирования", ex);
            }
        }

        public void SendBookingStatusUpdate(Bookings booking)
        {
            try
            {
                var customer = booking.Customers;
                var hotel = booking.Rooms.Hotels;

                var subject = "Обновление статуса бронирования";
                var body = $@"
Уважаемый(ая) {customer.FirstName} {customer.LastName},

Статус вашего бронирования в отеле {hotel.HotelName} изменен.

Номер бронирования: {booking.BookingID}
Новый статус: {booking.Status}

Детали бронирования:
Номер: {booking.Rooms.RoomNumber}
Дата заезда: {booking.CheckInDate.ToShortDateString()}
Дата выезда: {booking.CheckOutDate.ToShortDateString()}

Если у вас есть вопросы, пожалуйста, свяжитесь с нами.
";

                SendEmail(customer.Email, subject, body);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при отправке уведомления об обновлении статуса", ex);
            }
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(_smtpUsername, _smtpPassword);
                    client.EnableSsl = true;

                    var message = new MailMessage(_fromEmail, toEmail, subject, body);
                    message.IsBodyHtml = false;

                    client.Send(message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при отправке email", ex);
            }
        }
    }
} 