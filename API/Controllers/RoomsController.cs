using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using HotelsApp.Model;
using HotelsApp.API.DTOs;

namespace HotelsApp.API.Controllers
{
    [RoutePrefix("api/rooms")]
    public class RoomsController : ApiController
    {
        private readonly ProHotelEntities _dbContext;

        public RoomsController()
        {
            _dbContext = new ProHotelEntities();
        }

        // GET: api/rooms
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetRooms(int? hotelId = null)
        {
            try
            {
                IQueryable<Rooms> query = _dbContext.Rooms;

                if (hotelId.HasValue)
                {
                    query = query.Where(r => r.HotelID == hotelId.Value);
                }

                var rooms = query.Select(r => new RoomDTO
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
                        MaxOccupancy = r.RoomTypes.MaxOccupancy
                    } : null,
                    Amenities = r.Amenities.Select(a => new AmenityDTO
                    {
                        AmenityID = a.AmenityID,
                        AmenityName = a.AmenityName,
                        Description = a.Description
                    }).ToList()
                }).ToList();

                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/rooms/{id}
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetRoom(int id)
        {
            try
            {
                var room = _dbContext.Rooms.Find(id);
                if (room == null)
                {
                    return NotFound();
                }

                var roomDTO = new RoomDTO
                {
                    RoomID = room.RoomID,
                    HotelID = room.HotelID,
                    RoomNumber = room.RoomNumber,
                    Floor = room.Floor,
                    IsAvailable = room.IsAvailable,
                    RoomType = room.RoomTypes != null ? new RoomTypeDTO
                    {
                        RoomTypeID = room.RoomTypes.RoomTypeID,
                        TypeName = room.RoomTypes.TypeName,
                        Description = room.RoomTypes.Description,
                        BasePrice = room.RoomTypes.BasePrice,
                        MaxOccupancy = room.RoomTypes.MaxOccupancy
                    } : null,
                    Amenities = room.Amenities?.Select(a => new AmenityDTO
                    {
                        AmenityID = a.AmenityID,
                        AmenityName = a.AmenityName,
                        Description = a.Description
                    }).ToList()
                };

                return Ok(roomDTO);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/rooms/available
        [HttpGet]
        [Route("available")]
        public IHttpActionResult GetAvailableRooms(DateTime checkIn, DateTime checkOut, int? hotelId = null)
        {
            try
            {
                if (checkIn >= checkOut)
                {
                    return BadRequest("Дата выезда должна быть позже даты заезда");
                }

                IQueryable<Rooms> query = _dbContext.Rooms
                    .Where(r => r.IsAvailable == true &&
                        !r.Bookings.Any(b => b.Status != "Cancelled" &&
                            b.CheckInDate < checkOut && b.CheckOutDate > checkIn));

                if (hotelId.HasValue)
                {
                    query = query.Where(r => r.HotelID == hotelId.Value);
                }

                var rooms = query.Select(r => new RoomDTO
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
                        MaxOccupancy = r.RoomTypes.MaxOccupancy
                    } : null,
                    Amenities = r.Amenities.Select(a => new AmenityDTO
                    {
                        AmenityID = a.AmenityID,
                        AmenityName = a.AmenityName,
                        Description = a.Description
                    }).ToList()
                }).ToList();

                return Ok(rooms);
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

