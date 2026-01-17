using System;

namespace HotelsApp.API.DTOs
{
    public class BookingDTO
    {
        public int BookingID { get; set; }
        public int CustomerID { get; set; }
        public int RoomID { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public HotelDTO Hotel { get; set; }
        public RoomDTO Room { get; set; }
    }

    public class CreateBookingDTO
    {
        public int CustomerID { get; set; }
        public int RoomID { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }

    public class BookingListDTO
    {
        public int BookingID { get; set; }
        public string HotelName { get; set; }
        public string RoomNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
    }
}

