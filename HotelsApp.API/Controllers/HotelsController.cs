using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using HotelsApp.Model;
using HotelsApp.Services;
using HotelsApp.API.DTOs;

namespace HotelsApp.API.Controllers
{
    [RoutePrefix("api/hotels")]
    public class HotelsController : ApiController
    {
        private readonly HotelService _hotelService;
        private readonly ProHotelEntities _dbContext;

        public HotelsController()
        {
            _dbContext = new ProHotelEntities();
            _hotelService = new HotelService(_dbContext);
        }

        // GET: api/hotels
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetHotels(
            int? cityId = null,
            int? minStars = null,
            int? maxStars = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string amenities = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "Rating",
            bool ascending = false)
        {
            try
            {
                List<int> amenityIds = null;
                if (!string.IsNullOrEmpty(amenities))
                {
                    amenityIds = amenities.Split(',').Select(int.Parse).ToList();
                }

                var (hotels, totalCount) = _hotelService.SearchHotels(
                    cityId, minStars, maxStars, minPrice, maxPrice,
                    amenityIds, page, pageSize, sortBy, ascending);

                var hotelDTOs = hotels.Select(h => new HotelListDTO
                {
                    HotelID = h.HotelID,
                    HotelName = h.HotelName,
                    StarRating = h.StarRating,
                    Description = h.Description,
                    HotelImage = h.HotelImage,
                    CityName = h.Addresses?.Cities?.CityName,
                    MinPrice = h.Rooms?.Min(r => r.RoomTypes?.BasePrice)
                }).ToList();

                return Ok(new
                {
                    data = hotelDTOs,
                    totalCount = totalCount,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/hotels/{id}
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetHotel(int id)
        {
            try
            {
                var hotel = _dbContext.Hotels.Find(id);
                if (hotel == null)
                {
                    return NotFound();
                }

                var hotelDTO = new HotelDTO
                {
                    HotelID = hotel.HotelID,
                    HotelName = hotel.HotelName,
                    StarRating = hotel.StarRating,
                    Description = hotel.Description,
                    PhoneNumber = hotel.PhoneNumber,
                    Email = hotel.Email,
                    HotelImage = hotel.HotelImage,
                    Address = hotel.Addresses != null ? new AddressDTO
                    {
                        AddressID = hotel.Addresses.AddressID,
                        Street = hotel.Addresses.Street,
                        CityName = hotel.Addresses.Cities?.CityName,
                        CountryName = hotel.Addresses.Cities?.Countries?.CountryName
                    } : null,
                    Rooms = hotel.Rooms?.Select(r => new RoomDTO
                    {
                        RoomID = r.RoomID,
                        HotelID = r.HotelID,
                        RoomNumber = r.RoomNumber,
                        Floor = r.Floor,
                        IsAvailable = r.IsAvailable,
                        RoomType = r.RoomTypes != null ? new RoomTypeDTO
                        {
                            RoomTypeID = r.RoomTypes.RoomTypeID,
                            TypeName = r.RoomTypes.TypeName,
                            Description = r.RoomTypes.Description,
                            BasePrice = r.RoomTypes.BasePrice,
                            Capacity = r.RoomTypes.Capacity
                        } : null,
                        Amenities = r.Amenities?.Select(a => new AmenityDTO
                        {
                            AmenityID = a.AmenityID,
                            AmenityName = a.AmenityName,
                            Description = a.Description
                        }).ToList()
                    }).ToList()
                };

                return Ok(hotelDTO);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/hotels/{id}/rooms
        [HttpGet]
        [Route("{id}/rooms")]
        public IHttpActionResult GetHotelRooms(int id, DateTime? checkIn = null, DateTime? checkOut = null)
        {
            try
            {
                var hotel = _dbContext.Hotels.Find(id);
                if (hotel == null)
                {
                    return NotFound();
                }

                var rooms = hotel.Rooms.AsQueryable();

                // Фильтр по доступности, если указаны даты
                if (checkIn.HasValue && checkOut.HasValue)
                {
                    rooms = rooms.Where(r => r.IsAvailable == true &&
                        !r.Bookings.Any(b => b.Status != "Cancelled" &&
                            b.CheckInDate < checkOut && b.CheckOutDate > checkIn));
                }

                var roomDTOs = rooms.Select(r => new RoomDTO
                {
                    RoomID = r.RoomID,
                    HotelID = r.HotelID,
                    RoomNumber = r.RoomNumber,
                    Floor = r.Floor,
                    IsAvailable = r.IsAvailable,
                    RoomType = r.RoomTypes != null ? new RoomTypeDTO
                    {
                        RoomTypeID = r.RoomTypes.RoomTypeID,
                        TypeName = r.RoomTypes.TypeName,
                        Description = r.RoomTypes.Description,
                        BasePrice = r.RoomTypes.BasePrice,
                        Capacity = r.RoomTypes.Capacity
                    } : null,
                    Amenities = r.Amenities.Select(a => new AmenityDTO
                    {
                        AmenityID = a.AmenityID,
                        AmenityName = a.AmenityName,
                        Description = a.Description
                    }).ToList()
                }).ToList();

                return Ok(roomDTOs);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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

