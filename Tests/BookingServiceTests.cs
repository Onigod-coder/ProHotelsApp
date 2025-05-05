using System;
using System.Linq;
using Xunit;
using HotelsApp.Services;
using HotelsApp.Model;
using Moq;
using System.Collections.Generic;

namespace HotelsApp.Tests
{
    public class BookingServiceTests
    {
        private readonly Mock<Entities> _mockDbContext;
        private readonly Mock<NotificationService> _mockNotificationService;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _mockDbContext = new Mock<Entities>();
            _mockNotificationService = new Mock<NotificationService>("smtp.server", 587, "user", "pass", "from@email.com");
            _bookingService = new BookingService(_mockDbContext.Object, _mockNotificationService.Object);
        }

        [Fact]
        public void CreateBooking_WithValidParameters_CreatesBooking()
        {
            // Arrange
            var customerId = 1;
            var roomId = 1;
            var checkInDate = DateTime.Now.AddDays(1);
            var checkOutDate = DateTime.Now.AddDays(3);
            var room = new Rooms
            {
                RoomID = roomId,
                IsAvailable = true,
                RoomTypes = new RoomTypes { BasePrice = 100 }
            };

            var bookings = new List<Bookings>();
            _mockDbContext.Setup(x => x.Rooms.Find(roomId)).Returns(room);
            _mockDbContext.Setup(x => x.Bookings.Add(It.IsAny<Bookings>()))
                .Callback<Bookings>(b => bookings.Add(b));

            // Act
            var booking = _bookingService.CreateBooking(customerId, roomId, checkInDate, checkOutDate);

            // Assert
            Assert.Single(bookings);
            Assert.Equal(customerId, booking.CustomerID);
            Assert.Equal(roomId, booking.RoomID);
            Assert.Equal(checkInDate, booking.CheckInDate);
            Assert.Equal(checkOutDate, booking.CheckOutDate);
            Assert.Equal(200, booking.TotalPrice); // 2 ночи * 100
            Assert.Equal("Confirmed", booking.Status);
            _mockNotificationService.Verify(x => x.SendBookingConfirmation(It.IsAny<Bookings>()), Times.Once);
        }

        [Fact]
        public void CreateBooking_WithUnavailableRoom_ThrowsException()
        {
            // Arrange
            var room = new Rooms { RoomID = 1, IsAvailable = false };
            _mockDbContext.Setup(x => x.Rooms.Find(1)).Returns(room);

            // Act & Assert
            Assert.Throws<Exception>(() => 
                _bookingService.CreateBooking(1, 1, DateTime.Now, DateTime.Now.AddDays(1)));
        }

        [Fact]
        public void CancelBooking_WithValidBooking_CancelsBooking()
        {
            // Arrange
            var bookingId = 1;
            var booking = new Bookings
            {
                BookingID = bookingId,
                Status = "Confirmed"
            };

            _mockDbContext.Setup(x => x.Bookings.Find(bookingId)).Returns(booking);

            // Act
            _bookingService.CancelBooking(bookingId);

            // Assert
            Assert.Equal("Cancelled", booking.Status);
            _mockNotificationService.Verify(x => x.SendBookingCancellation(It.IsAny<Bookings>()), Times.Once);
        }

        [Fact]
        public void CancelBooking_WithAlreadyCancelledBooking_ThrowsException()
        {
            // Arrange
            var booking = new Bookings { Status = "Cancelled" };
            _mockDbContext.Setup(x => x.Bookings.Find(1)).Returns(booking);

            // Act & Assert
            Assert.Throws<Exception>(() => _bookingService.CancelBooking(1));
        }

        [Fact]
        public void GetCustomerBookings_WithValidCustomerId_ReturnsBookings()
        {
            // Arrange
            var customerId = 1;
            var bookings = new List<Bookings>
            {
                new Bookings { BookingID = 1, CustomerID = customerId, BookingDate = DateTime.Now.AddDays(-1) },
                new Bookings { BookingID = 2, CustomerID = customerId, BookingDate = DateTime.Now }
            };

            _mockDbContext.Setup(x => x.Bookings).Returns(bookings.AsQueryable());

            // Act
            var result = _bookingService.GetCustomerBookings(customerId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(2, result[0].BookingID); // Проверяем сортировку по дате
        }

        [Fact]
        public void UpdateBookingStatus_WithValidParameters_UpdatesStatus()
        {
            // Arrange
            var bookingId = 1;
            var newStatus = "Completed";
            var booking = new Bookings { BookingID = bookingId, Status = "Confirmed" };

            _mockDbContext.Setup(x => x.Bookings.Find(bookingId)).Returns(booking);

            // Act
            _bookingService.UpdateBookingStatus(bookingId, newStatus);

            // Assert
            Assert.Equal(newStatus, booking.Status);
            _mockNotificationService.Verify(x => x.SendBookingStatusUpdate(It.IsAny<Bookings>()), Times.Once);
        }

        [Fact]
        public void UpdateBookingStatus_WithNonExistentBooking_ThrowsException()
        {
            // Arrange
            _mockDbContext.Setup(x => x.Bookings.Find(1)).Returns((Bookings)null);

            // Act & Assert
            Assert.Throws<Exception>(() => _bookingService.UpdateBookingStatus(1, "Completed"));
        }
    }
} 