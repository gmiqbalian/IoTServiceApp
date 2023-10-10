using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IoTServiceApp.Services
{
    public class DateAndTimeService
    {
        private readonly Timer _timer;
        public event Action? TimeUpdated;
        public string? Date { get; private set; }
        public string? Time { get; private set; }


        public DateAndTimeService() 
        {
            GetDateAndTime();

            _timer = new Timer(1000);
            _timer.Elapsed += (s, e) => GetDateAndTime();
            _timer.Start();
        }

        private void GetDateAndTime()
        {
            Date = DateTime.Now.ToString("HH:mm");
            Time = DateTime.Now.ToString("dddd, d MMMM yyyy");

            TimeUpdated?.Invoke();
        }
    }
}
