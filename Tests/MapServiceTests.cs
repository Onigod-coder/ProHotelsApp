using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using HotelsApp.Services;
using Moq;

namespace HotelsApp.Tests
{
    public class MapServiceTests
    {
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly MapService _mapService;
        private const string TestApiKey = "test_api_key";

        public MapServiceTests()
        {
            _mockHttpClient = new Mock<HttpClient>();
            _mapService = new MapService(TestApiKey);
        }

        [Fact]
        public async Task GetCoordinates_WithValidAddress_ReturnsCoordinates()
        {
            // Arrange
            var address = "Москва, Красная площадь, 1";
            var expectedLatitude = 55.7539;
            var expectedLongitude = 37.6208;

            var response = $@"{{
                ""results"": [
                    {{
                        ""geometry"": {{
                            ""location"": {{
                                ""lat"": {expectedLatitude},
                                ""lng"": {expectedLongitude}
                            }}
                        }}
                    }}
                ]
            }}";

            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var coordinates = await _mapService.GetCoordinates(address);

            // Assert
            Assert.Equal(expectedLatitude, coordinates.Latitude);
            Assert.Equal(expectedLongitude, coordinates.Longitude);
        }

        [Fact]
        public async Task GetCoordinates_WithInvalidAddress_ThrowsException()
        {
            // Arrange
            var address = "Invalid Address";
            var response = @"{""results"": []}";

            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _mapService.GetCoordinates(address));
        }

        [Fact]
        public async Task GetStaticMapUrl_WithValidParameters_ReturnsValidUrl()
        {
            // Arrange
            var address = "Москва, Красная площадь, 1";
            var width = 800;
            var height = 600;
            var zoom = 15;

            // Act
            var mapUrl = await _mapService.GetStaticMapUrl(address, width, height, zoom);

            // Assert
            Assert.NotNull(mapUrl);
            Assert.Contains($"size={width}x{height}", mapUrl);
            Assert.Contains($"zoom={zoom}", mapUrl);
            Assert.Contains(TestApiKey, mapUrl);
        }

        [Fact]
        public async Task CalculateDistance_WithValidPoints_ReturnsDistance()
        {
            // Arrange
            var point1 = (55.7539, 37.6208); // Москва
            var point2 = (59.9343, 30.3351); // Санкт-Петербург
            var expectedDistance = 634.0; // Примерное расстояние в км

            var response = $@"{{
                ""rows"": [
                    {{
                        ""elements"": [
                            {{
                                ""distance"": {{
                                    ""value"": {expectedDistance * 1000}
                                }}
                            }}
                        ]
                    }}
                ]
            }}";

            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var distance = await _mapService.CalculateDistance(point1, point2);

            // Assert
            Assert.Equal(expectedDistance, distance, 1); // Проверяем с точностью до 1 км
        }

        [Fact]
        public async Task GetNearbyPlaces_WithValidParameters_ReturnsPlaces()
        {
            // Arrange
            var location = (55.7539, 37.6208);
            var type = "restaurant";
            var radius = 1000;

            var response = $@"{{
                ""results"": [
                    {{""name"": ""Ресторан 1""}},
                    {{""name"": ""Ресторан 2""}},
                    {{""name"": ""Ресторан 3""}}
                ]
            }}";

            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var places = await _mapService.GetNearbyPlaces(location, type, radius);

            // Assert
            Assert.Equal(3, places.Length);
            Assert.Contains("Ресторан 1", places);
            Assert.Contains("Ресторан 2", places);
            Assert.Contains("Ресторан 3", places);
        }

        [Fact]
        public async Task GetNearbyPlaces_WithNoResults_ReturnsEmptyArray()
        {
            // Arrange
            var location = (55.7539, 37.6208);
            var type = "restaurant";
            var radius = 1000;

            var response = @"{""results"": []}";

            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var places = await _mapService.GetNearbyPlaces(location, type, radius);

            // Assert
            Assert.Empty(places);
        }
    }
} 