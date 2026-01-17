using System.Collections.Generic;

namespace HotelsApp.API.DTOs
{
    public class HotelDTO
    {
        public int HotelID { get; set; }
        public string HotelName { get; set; }
        public int? StarRating { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string HotelImage { get; set; }
        public AddressDTO Address { get; set; }
        public List<RoomDTO> Rooms { get; set; }
    }

    public class AddressDTO
    {
        public int AddressID { get; set; }
        public string Street { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
    }

    public class HotelListDTO
    {
        public int HotelID { get; set; }
        public string HotelName { get; set; }
        public int? StarRating { get; set; }
        public string Description { get; set; }
        public string HotelImage { get; set; }
        public string CityName { get; set; }
        public decimal? MinPrice { get; set; }
    }
}

