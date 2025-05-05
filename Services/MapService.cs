using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using HotelsApp.Model;

namespace HotelsApp.Services
{
    public class MapService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public MapService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        public async Task<(double Latitude, double Longitude)> GetCoordinates(string address)
        {
            try
            {
                var encodedAddress = Uri.EscapeDataString(address);
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={_apiKey}";

                var response = await _httpClient.GetStringAsync(url);
                var result = JsonSerializer.Deserialize<GeocodingResponse>(response);

                if (result?.Results?.Length > 0)
                {
                    var location = result.Results[0].Geometry.Location;
                    return (location.Lat, location.Lng);
                }

                throw new Exception("Адрес не найден");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении координат", ex);
            }
        }

        public async Task<string> GetStaticMapUrl(string address, int width = 600, int height = 400, int zoom = 15)
        {
            try
            {
                var coordinates = await GetCoordinates(address);
                var encodedAddress = Uri.EscapeDataString(address);

                return $"https://maps.googleapis.com/maps/api/staticmap?" +
                       $"center={coordinates.Latitude},{coordinates.Longitude}" +
                       $"&zoom={zoom}" +
                       $"&size={width}x{height}" +
                       $"&markers=color:red%7C{coordinates.Latitude},{coordinates.Longitude}" +
                       $"&key={_apiKey}";
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении статической карты", ex);
            }
        }

        public async Task<double> CalculateDistance((double Lat, double Lng) point1, (double Lat, double Lng) point2)
        {
            try
            {
                var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?" +
                         $"origins={point1.Lat},{point1.Lng}" +
                         $"&destinations={point2.Lat},{point2.Lng}" +
                         $"&key={_apiKey}";

                var response = await _httpClient.GetStringAsync(url);
                var result = JsonSerializer.Deserialize<DistanceMatrixResponse>(response);

                if (result?.Rows?.Length > 0 && result.Rows[0].Elements?.Length > 0)
                {
                    return result.Rows[0].Elements[0].Distance.Value / 1000.0; // Конвертируем в километры
                }

                throw new Exception("Не удалось рассчитать расстояние");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при расчете расстояния", ex);
            }
        }

        public async Task<string[]> GetNearbyPlaces((double Lat, double Lng) location, string type, int radius = 1000)
        {
            try
            {
                var url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?" +
                         $"location={location.Lat},{location.Lng}" +
                         $"&radius={radius}" +
                         $"&type={type}" +
                         $"&key={_apiKey}";

                var response = await _httpClient.GetStringAsync(url);
                var result = JsonSerializer.Deserialize<PlacesResponse>(response);

                if (result?.Results?.Length > 0)
                {
                    return result.Results.Select(p => p.Name).ToArray();
                }

                return Array.Empty<string>();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении ближайших мест", ex);
            }
        }

        private class GeocodingResponse
        {
            public Result[] Results { get; set; }

            public class Result
            {
                public Geometry Geometry { get; set; }
            }

            public class Geometry
            {
                public Location Location { get; set; }
            }

            public class Location
            {
                public double Lat { get; set; }
                public double Lng { get; set; }
            }
        }

        private class DistanceMatrixResponse
        {
            public Row[] Rows { get; set; }

            public class Row
            {
                public Element[] Elements { get; set; }
            }

            public class Element
            {
                public Distance Distance { get; set; }
            }

            public class Distance
            {
                public int Value { get; set; }
            }
        }

        private class PlacesResponse
        {
            public Place[] Results { get; set; }

            public class Place
            {
                public string Name { get; set; }
            }
        }
    }
} 