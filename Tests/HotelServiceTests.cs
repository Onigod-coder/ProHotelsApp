using System;
using System.Linq;
using Xunit;
using HotelsApp.Services;
using HotelsApp.Model;
using Moq;
using System.Collections.Generic;

namespace HotelsApp.Tests
{
    public class HotelServiceTests
    {
        private readonly Mock<Entities> _mockDbContext;
        private readonly HotelService _hotelService;

        public HotelServiceTests()
        {
            _mockDbContext = new Mock<Entities>();
            _hotelService = new HotelService(_mockDbContext.Object);
        }

        [Fact]
        public void SearchHotels_WithValidParameters_ReturnsFilteredHotels()
        {
            // Arrange
            var hotels = new List<Hotels>
            {
                new Hotels { HotelID = 1, HotelName = "Hotel 1", StarRating = 4 },
                new Hotels { HotelID = 2, HotelName = "Hotel 2", StarRating = 5 },
                new Hotels { HotelID = 3, HotelName = "Hotel 3", StarRating = 3 }
            };

            _mockDbContext.Setup(x => x.Hotels).Returns(hotels.AsQueryable());

            // Act
            var result = _hotelService.SearchHotels(minStars: 4);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Hotels, h => Assert.True(h.StarRating >= 4));
        }

        [Fact]
        public void GetRecommendedHotels_WithUserHistory_ReturnsRelevantHotels()
        {
            // Arrange
            var customerId = 1;
            var userBookings = new List<Bookings>
            {
                new Bookings
                {
                    CustomerID = customerId,
                    Rooms = new Rooms
                    {
                        Hotels = new Hotels
                        {
                            HotelID = 1,
                            StarRating = 4,
                            Addresses = new Addresses { CityID = 1 }
                        }
                    }
                }
            };

            var allHotels = new List<Hotels>
            {
                new Hotels { HotelID = 2, StarRating = 4, Addresses = new Addresses { CityID = 1 } },
                new Hotels { HotelID = 3, StarRating = 5, Addresses = new Addresses { CityID = 1 } },
                new Hotels { HotelID = 4, StarRating = 3, Addresses = new Addresses { CityID = 2 } }
            };

            _mockDbContext.Setup(x => x.Bookings).Returns(userBookings.AsQueryable());
            _mockDbContext.Setup(x => x.Hotels).Returns(allHotels.AsQueryable());

            // Act
            var result = _hotelService.GetRecommendedHotels(customerId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, h => Assert.Equal(1, h.Addresses.CityID));
        }

        [Fact]
        public void AddToFavorites_WithValidParameters_AddsHotelToFavorites()
        {
            // Arrange
            var customerId = 1;
            var hotelId = 1;
            var favorites = new List<Favorites>();

            _mockDbContext.Setup(x => x.Favorites).Returns(favorites.AsQueryable());
            _mockDbContext.Setup(x => x.Favorites.Add(It.IsAny<Favorites>()))
                .Callback<Favorites>(f => favorites.Add(f));

            // Act
            _hotelService.AddToFavorites(customerId, hotelId);

            // Assert
            Assert.Single(favorites);
            Assert.Equal(customerId, favorites[0].CustomerID);
            Assert.Equal(hotelId, favorites[0].HotelID);
        }

        [Fact]
        public void RemoveFromFavorites_WithExistingFavorite_RemovesHotelFromFavorites()
        {
            // Arrange
            var customerId = 1;
            var hotelId = 1;
            var favorites = new List<Favorites>
            {
                new Favorites { CustomerID = customerId, HotelID = hotelId }
            };

            _mockDbContext.Setup(x => x.Favorites).Returns(favorites.AsQueryable());
            _mockDbContext.Setup(x => x.Favorites.Remove(It.IsAny<Favorites>()))
                .Callback<Favorites>(f => favorites.Remove(f));

            // Act
            _hotelService.RemoveFromFavorites(customerId, hotelId);

            // Assert
            Assert.Empty(favorites);
        }

        [Fact]
        public void GetFavoriteHotels_WithExistingFavorites_ReturnsFavoriteHotels()
        {
            // Arrange
            var customerId = 1;
            var favorites = new List<Favorites>
            {
                new Favorites
                {
                    CustomerID = customerId,
                    Hotels = new Hotels { HotelID = 1, StarRating = 4 }
                },
                new Favorites
                {
                    CustomerID = customerId,
                    Hotels = new Hotels { HotelID = 2, StarRating = 5 }
                }
            };

            _mockDbContext.Setup(x => x.Favorites).Returns(favorites.AsQueryable());

            // Act
            var result = _hotelService.GetFavoriteHotels(customerId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(5, result[0].StarRating); // Проверяем сортировку по рейтингу
        }
    }
} 