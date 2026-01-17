using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using HotelsApp.Model;

namespace HotelsApp.Services
{
    public class BookingService
    {
        private readonly ProHotelEntities _dbContext;
        private readonly NotificationService _notificationService;

        public BookingService(ProHotelEntities dbContext, NotificationService notificationService)
        {
            _dbContext = dbContext;
            _notificationService = notificationService;
        }

        public Bookings CreateBooking(int customerId, int roomId, DateTime checkInDate, DateTime checkOutDate)
        {
            try
            {
                var room = _dbContext.Rooms.Find(roomId);
                if (room == null)
                    throw new Exception("Номер не найден");

                if (!room.IsAvailable.HasValue || !room.IsAvailable.Value)
                    throw new Exception("Номер недоступен для бронирования");

                var nights = (checkOutDate - checkInDate).Days;
                var totalPrice = room.RoomTypes.BasePrice * nights;

                var booking = new Bookings
                {
                    CustomerID = customerId,
                    RoomID = roomId,
                    BookingDate = DateTime.Now,
                    CheckInDate = checkInDate,
                    CheckOutDate = checkOutDate,
                    TotalPrice = totalPrice,
                    Status = "Confirmed"
                };

                _dbContext.Bookings.Add(booking);
                _dbContext.SaveChanges();

                // Отправляем уведомление
                _notificationService.SendBookingConfirmation(booking);

                return booking;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при создании бронирования", ex);
            }
        }

        public void CancelBooking(int bookingId)
        {
            try
            {
                var booking = _dbContext.Bookings.Find(bookingId);
                if (booking == null)
                    throw new Exception("Бронирование не найдено");

                if (booking.Status == "Cancelled")
                    throw new Exception("Бронирование уже отменено");

                booking.Status = "Cancelled";
                _dbContext.SaveChanges();

                // Отправляем уведомление
                _notificationService.SendBookingCancellation(booking);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при отмене бронирования", ex);
            }
        }

        public List<Bookings> GetCustomerBookings(int customerId)
        {
            try
            {
                return _dbContext.Bookings
                    .Where(b => b.CustomerID == customerId)
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении бронирований", ex);
            }
        }

        public void ExportBookingToPdf(int bookingId, string filePath)
        {
            try
            {
                var booking = _dbContext.Bookings.Find(bookingId);
                if (booking == null)
                    throw new Exception("Бронирование не найдено");

                using (var document = new Document())
                {
                    using (var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create)))
                    {
                        document.Open();

                        // Заголовок
                        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                        var title = new Paragraph("Подтверждение бронирования", titleFont);
                        title.Alignment = Element.ALIGN_CENTER;
                        document.Add(title);

                        document.Add(new Paragraph("\n"));

                        // Информация о бронировании
                        var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                        document.Add(new Paragraph($"Номер бронирования: {booking.BookingID}", normalFont));
                        document.Add(new Paragraph($"Отель: {booking.Rooms.Hotels.HotelName}", normalFont));
                        document.Add(new Paragraph($"Номер: {booking.Rooms.RoomNumber}", normalFont));
                        document.Add(new Paragraph($"Дата заезда: {booking.CheckInDate.ToShortDateString()}", normalFont));
                        document.Add(new Paragraph($"Дата выезда: {booking.CheckOutDate.ToShortDateString()}", normalFont));
                        document.Add(new Paragraph($"Стоимость: {booking.TotalPrice:C}", normalFont));
                        document.Add(new Paragraph($"Статус: {booking.Status}", normalFont));

                        document.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при экспорте бронирования в PDF", ex);
            }
        }

        public void UpdateBookingStatus(int bookingId, string status)
        {
            try
            {
                var booking = _dbContext.Bookings.Find(bookingId);
                if (booking == null)
                    throw new Exception("Бронирование не найдено");

                booking.Status = status;
                _dbContext.SaveChanges();

                // Отправляем уведомление об изменении статуса
                _notificationService.SendBookingStatusUpdate(booking);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при обновлении статуса бронирования", ex);
            }
        }
    }
} 