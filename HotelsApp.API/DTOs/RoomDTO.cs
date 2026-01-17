using System.Collections.Generic;

namespace HotelsApp.API.DTOs
{
    public class RoomDTO
    {
        public int RoomID { get; set; }
        public int HotelID { get; set; }
        public string RoomNumber { get; set; }
        public int Floor { get; set; }
        public bool? IsAvailable { get; set; }
        public RoomTypeDTO RoomType { get; set; }
        public List<AmenityDTO> Amenities { get; set; }
    }

    public class RoomTypeDTO
    {
        public int RoomTypeID { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int Capacity { get; set; }
    }

    public class AmenityDTO
    {
        public int AmenityID { get; set; }
        public string AmenityName { get; set; }
        public string Description { get; set; }
    }
}

