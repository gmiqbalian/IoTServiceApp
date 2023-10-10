using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IoTServiceApp.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _outsideTempUrl;
        private readonly Timer _timer;
        public string? OutsideTemp {  get; set; }
        public string? WeatherIcon { get; set; }
        public event Action WeatheUpdated;
        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            Task.Run(GetWeatherDataAsync);

            _timer = new Timer(60000 * 15);
            _timer.Elapsed += (s, e) => GetWeatherDataAsync();
            _timer.Start();

        }
        private async Task GetWeatherDataAsync()
        {
            try
            {
                var response = await _httpClient
                    .GetStringAsync("https://api.openweathermap.org/data/2.5/weather?lat=59.334591&lon=18.063240&appid=078db5ab801b77dd0a2fdfba926a61c7&units=metric");
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

            WeatheUpdated.Invoke();

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
