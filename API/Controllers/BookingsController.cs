using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using HotelsApp.Model;
using HotelsApp.Services;
using HotelsApp.API.DTOs;
using System.Configuration;

namespace HotelsApp.API.Controllers
{
    [RoutePrefix("api/bookings")]
    public class BookingsController : ApiController
    {
        private readonly BookingService _bookingService;
        private readonly ProHotelEntities _dbContext;

        public BookingsController()
        {
            _dbContext = new ProHotelEntities();
            var notificationService = new NotificationService(
                ConfigurationManager.AppSettings["SmtpServer"],
                int.Parse(ConfigurationManager.AppSettings["SmtpPort"]),
                ConfigurationManager.AppSettings["SmtpUsername"],
                ConfigurationManager.AppSettings["SmtpPassword"],
                ConfigurationManager.AppSettings["FromEmail"]
            );
            _bookingService = new BookingService(_dbContext, notificationService);
        }

        // GET: api/bookings
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetBookings(int? customerId = null)
        {
            try
            {
                IQueryable<Bookings> query = _dbContext.Bookings;

                if (customerId.HasValue)
                {
                    query = query.Where(b => b.CustomerID == customerId.Value);
                }

                var bookings = query.OrderByDescending(b => b.BookingDate)
                    .Select(b => new BookingListDTO
                    {
                        BookingID = b.BookingID,
                        HotelName = b.Rooms.Hotels.HotelName,
                        RoomNumber = b.Rooms.RoomNumber,
                        CheckInDate = b.CheckInDate,
                        CheckOutDate = b.CheckOutDate,
                        TotalPrice = b.TotalPrice,
                        Status = b.Status
                    }).ToList();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/bookings/{id}
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetBooking(int id)
        {
            try
            {
                var booking = _dbContext.Bookings.Find(id);
                if (booking == null)
                {
                    return NotFound();
                }

                var bookingDTO = new BookingDTO
                {
                    BookingID = booking.BookingID,
                    CustomerID = booking.CustomerID,
                    RoomID = booking.RoomID,
                    BookingDate = booking.BookingDate,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    TotalPrice = booking.TotalPrice,
                    Status = booking.Status,
                    Hotel = booking.Rooms?.Hotels != null ? new HotelDTO
                    {
                        HotelID = booking.Rooms.Hotels.HotelID,
                        HotelName = booking.Rooms.Hotels.HotelName,
                        StarRating = booking.Rooms.Hotels.StarRating,
                        Description = booking.Rooms.Hotels.Description,
                        PhoneNumber = booking.Rooms.Hotels.PhoneNumber,
                        Email = booking.Rooms.Hotels.Email,
                        HotelImage = booking.Rooms.Hotels.HotelImage
                    } : null,
                    Room = booking.Rooms != null ? new RoomDTO
                    {
                        RoomID = booking.Rooms.RoomID,
                        HotelID = booking.Rooms.HotelID,
                        RoomNumber = booking.Rooms.RoomNumber,
                        Floor = booking.Rooms.Floor,
                        IsAvailable = booking.Rooms.IsAvailable
                    } : null
                };

                return Ok(bookingDTO);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/bookings
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateBooking([FromBody] CreateBookingDTO bookingDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (bookingDTO.CheckInDate >= bookingDTO.CheckOutDate)
                {
                    return BadRequest("Дата выезда должна быть позже даты заезда");
                }

                var booking = _bookingService.CreateBooking(
                    bookingDTO.CustomerID,
                    bookingDTO.RoomID,
                    bookingDTO.CheckInDate,
                    bookingDTO.CheckOutDate
                );

                var result = new BookingDTO
                {
                    BookingID = booking.BookingID,
                    CustomerID = booking.CustomerID,
                    RoomID = booking.RoomID,
                    BookingDate = booking.BookingDate,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    TotalPrice = booking.TotalPrice,
                    Status = booking.Status
                };

                return Created($"api/bookings/{booking.BookingID}", result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/bookings/{id}/cancel
        [HttpPut]
        [Route("{id}/cancel")]
        public IHttpActionResult CancelBooking(int id)
        {
            try
            {
                _bookingService.CancelBooking(id);
                return Ok(new { message = "Бронирование отменено" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

