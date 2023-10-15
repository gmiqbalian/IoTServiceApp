using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemTimer = System.Timers.Timer;

namespace IoTServiceAppLibrary.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _outsideTempUrl;
        private readonly SystemTimer _timer;
        private readonly IConfiguration _configuration;
        public string? OutsideTemp { get; set; }
        public string? WeatherIcon { get; set; }
        public event Action? WeatheUpdated;
        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _outsideTempUrl = _configuration.GetConnectionString("weatherUrl")!;

            Task.Run(GetWeatherDataAsync);

            _timer = new System.Timers.Timer(60000 * 15);
            _timer.Elapsed += (s, e) => GetWeatherDataAsync();
            _timer.Start();

        }
        private async Task GetWeatherDataAsync()
        {
            try
            {
                var response = await _httpClient
                    .GetStringAsync(_outsideTempUrl);
                var tempratureData = JsonConvert.DeserializeObject<dynamic>(response);

                OutsideTemp = tempratureData?.main.temp.ToString("#");
                WeatherIcon = GetWeatherIcon(tempratureData?.weather[0].description.ToString());
            }
            catch (Exception ex)
            {
                OutsideTemp = "--";
                WeatherIcon = GetWeatherIcon("--");

                Debug.WriteLine(ex.Message);
            }

            WeatheUpdated?.Invoke();

        }

        private string? GetWeatherIcon(string condition)
        {
            return condition switch
            {
                "clear sky" => "\ue28f",
                "few clouds" => "\uf6c4",
                "scattered clouds" => "\uf0c2",
                "broken clouds" => "\uf744",
                "shower rain" => "\uf738",
                "rain" => "\uf740",
                "thunderstorm" => "\uf76c",
                "snow" => "\uf742",
                "mist" => "\uf74e",
                "overcast clouds" => "\uf744",
                _ => "\ue137",
            };
        }
    }
}
