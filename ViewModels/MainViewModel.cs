using ClockSystem.WinUI.Models;
using ClockSystem.WinUI.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClockSystem.WinUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ConfigService _configService;
        private readonly ClockService _clockService;
        private DateTime _masterTime;
        private bool _useSystemTime = true;
        private double _timeOffset = 0;
        private double _highPrecisionOffset = 0;
        private System.Diagnostics.Stopwatch _precisionStopwatch;
        private DateTime _lastSyncTime;
        private string _logText = "";

        public event Action<string> LogMessageReceived;

        public DateTime MasterTime
        {
            get => _masterTime;
            set
            {
                if (_masterTime != value)
                {
                    _masterTime = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TimeDisplay));
                }
            }
        }

        public DateTime HighPrecisionTime => _masterTime.AddMilliseconds(_highPrecisionOffset);

        public string TimeDisplay => MasterTime.ToString("yyyy-MM-dd HH:mm:ss");

        public string LogText
        {
            get => _logText;
            set
            {
                if (_logText != value)
                {
                    _logText = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool UseSystemTime
        {
            get => _useSystemTime;
            set => _useSystemTime = value;
        }

        public bool IsLightOn => _clockService.LightOn;

        public RelayCommand SyncSystemTimeCommand { get; set; }
        public RelayCommand OpenTimeAdjustCommand { get; set; }
        public RelayCommand OpenSettingsCommand { get; set; }

        public event Action OpenTimeAdjustDialog;
        public event Action OpenSettingsDialog;

        public MainViewModel()
        {
            _configService = new ConfigService();
            var audioService = new AudioService();
            _clockService = new ClockService(_configService, audioService);

            _clockService.LogMessage += (message) =>
            {
                LogText += message + "\n";
                LogMessageReceived?.Invoke(message);
            };

            MasterTime = DateTime.Now;
            _clockService.SetTimeProvider(() => MasterTime);
            _clockService.Start();

            StartTimeUpdate();

            SyncSystemTimeCommand = new RelayCommand(_ => SyncSystemTime());
            OpenTimeAdjustCommand = new RelayCommand(_ => OpenTimeAdjustDialog?.Invoke());
            OpenSettingsCommand = new RelayCommand(_ => OpenSettingsDialog?.Invoke());
        }

        private void StartTimeUpdate()
        {
            Task.Run(() =>
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                _precisionStopwatch = System.Diagnostics.Stopwatch.StartNew();
                _lastSyncTime = DateTime.Now;
                var lastSecond = -1;
                
                while (true)
                {
                    try
                    {
                        var currentTime = DateTime.Now;
                        var elapsed = stopwatch.Elapsed.TotalMilliseconds;
                        
                        if (UseSystemTime)
                        {
                            MasterTime = currentTime;
                        }
                        else
                        {
                            MasterTime = currentTime.AddSeconds(_timeOffset);
                        }
                        
                        _highPrecisionOffset = elapsed;
                        
                        if (currentTime.Second != lastSecond)
                        {
                            lastSecond = currentTime.Second;
                            _lastSyncTime = currentTime;
                        }
                        
                        OnPropertyChanged(nameof(HighPrecisionTime));
                        
                        var targetElapsed = 16;
                        var actualElapsed = stopwatch.Elapsed.TotalMilliseconds;
                        var delay = Math.Max(1, (int)(targetElapsed - actualElapsed));
                        
                        stopwatch.Restart();
                        Thread.Sleep(delay);
                    }
                    catch
                    {
                    }
                }
            });
        }

        public void SyncSystemTime()
        {
            UseSystemTime = true;
            _timeOffset = 0;
            MasterTime = DateTime.Now;
        }

        public void AdjustTime(DateTime newTime)
        {
            UseSystemTime = false;
            MasterTime = newTime;
            _timeOffset = (newTime - DateTime.Now).TotalSeconds;
        }

        public ConfigModel LoadConfig()
        {
            return _configService.LoadConfig();
        }

        public void SaveConfig(ConfigModel config)
        {
            _configService.SaveConfig(config);
        }
    }
}
