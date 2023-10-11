using SystemTimer = System.Timers.Timer;

namespace IoTServiceAppLibrary.Services
{
    public class DateAndTimeService
    {
        private readonly SystemTimer _timer;
        public event Action? TimeUpdated;
        public string? Date { get; private set; }
        public string? Time { get; private set; }


        public DateAndTimeService()
        {
            GetDateAndTime();

            _timer = new System.Timers.Timer(1000);
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
