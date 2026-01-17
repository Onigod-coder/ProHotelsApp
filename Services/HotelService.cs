using System;
using System.Collections.Generic;
using System.Linq;
using HotelsApp.Model;

namespace HotelsApp.Services
{
    public class HotelService
    {
        private readonly ProHotelEntities _dbContext;

        public HotelService(ProHotelEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public (List<Hotels> Hotels, int TotalCount) SearchHotels(
            int? cityId = null,
            int? minStars = null,
            int? maxStars = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            List<int> amenities = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "Rating",
            bool ascending = false)
        {
            try
            {
                var query = _dbContext.Hotels.AsQueryable();

                // Фильтрация
                if (cityId.HasValue)
                    query = query.Where(h => h.Addresses.CityID == cityId);

                if (minStars.HasValue)
                    query = query.Where(h => h.StarRating >= minStars);

                if (maxStars.HasValue)
                    query = query.Where(h => h.StarRating <= maxStars);

                if (minPrice.HasValue)
                    query = query.Where(h => h.Rooms.Any(r => r.RoomTypes.BasePrice >= minPrice));

                if (maxPrice.HasValue)
                    query = query.Where(h => h.Rooms.Any(r => r.RoomTypes.BasePrice <= maxPrice));

                if (amenities != null && amenities.Any())
                    query = query.Where(h => h.Rooms.Any(r => r.Amenities.Any(a => amenities.Contains(a.AmenityID))));

                // Сортировка
                var sortByLower = sortBy.ToLower();
                if (sortByLower == "price")
                {
                    query = ascending ? query.OrderBy(h => h.Rooms.Min(r => r.RoomTypes.BasePrice))
                                     : query.OrderByDescending(h => h.Rooms.Min(r => r.RoomTypes.BasePrice));
                }
                else if (sortByLower == "name")
                {
                    query = ascending ? query.OrderBy(h => h.HotelName)
                                     : query.OrderByDescending(h => h.HotelName);
                }
                else
                {
                    query = ascending ? query.OrderBy(h => h.StarRating)
                                     : query.OrderByDescending(h => h.StarRating);
                }

                // Пагинация
                var totalCount = query.Count();
                var hotels = query.Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

                return (hotels, totalCount);
            }
            catch (Exception ex)
            {
                // Логирование ошибки будет реализовано позже
                throw new Exception("Ошибка при поиске отелей", ex);
            }
        }

        public List<Hotels> GetRecommendedHotels(int customerId, int count = 5)
        {
            try
            {
                // Получаем историю бронирований пользователя
                var userBookings = _dbContext.Bookings
                    .Where(b => b.CustomerID == customerId)
                    .Select(b => b.Rooms.Hotels)
                    .ToList();

                // Получаем предпочтения пользователя (города, типы отелей)
                var preferredCities = userBookings.Select(h => h.Addresses.CityID).Distinct();
                var preferredStarRating = userBookings.Where(h => h.StarRating.HasValue).Average(h => h.StarRating.Value);

                // Ищем похожие отели
                return _dbContext.Hotels
                    .Where(h => preferredCities.Contains(h.Addresses.CityID) &&
                               h.StarRating.HasValue &&
                               Math.Abs(h.StarRating.Value - preferredStarRating) <= 1)
                    .OrderByDescending(h => h.StarRating)
                    .Take(count)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении рекомендаций", ex);
            }
        }

        // Методы для работы с избранным отключены, так как модель Favorites отсутствует в базе данных
        // Раскомментируйте после добавления таблицы Favorites в базу данных
        
        /*
        public void AddToFavorites(int customerId, int hotelId)
        {
            try
            {
                var favorite = new Favorites
                {
                    CustomerID = customerId,
                    HotelID = hotelId,
                    AddedDate = DateTime.Now
                };

                _dbContext.Favorites.Add(favorite);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при добавлении в избранное", ex);
            }
        }

        public void RemoveFromFavorites(int customerId, int hotelId)
        {
            try
            {
                var favorite = _dbContext.Favorites
                    .FirstOrDefault(f => f.CustomerID == customerId && f.HotelID == hotelId);

                if (favorite != null)
                {
                    _dbContext.Favorites.Remove(favorite);
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при удалении из избранного", ex);
            }
        }

        public List<Hotels> GetFavoriteHotels(int customerId)
        {
            try
            {
                return _dbContext.Favorites
                    .Where(f => f.CustomerID == customerId)
                    .Select(f => f.Hotels)
                    .OrderByDescending(h => h.StarRating)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении избранных отелей", ex);
            }
        }
        */
    }
} 