using ClockSystem.WinUI.Dialogs;
using ClockSystem.WinUI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;

namespace ClockSystem.WinUI.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; private set; }
        private DispatcherTimer _clockTimer;

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new MainViewModel();
            DataContext = ViewModel;
            
            ViewModel.LogMessageReceived += OnLogMessageReceived;
            
            _clockTimer = new DispatcherTimer();
            _clockTimer.Interval = TimeSpan.FromMilliseconds(16);
            _clockTimer.Tick += (s, e) => DrawClock();
            _clockTimer.Start();
        }

        private void OnLogMessageReceived(string message)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                LogTextBlock.Text = ViewModel.LogText;
            });
        }

        private void DrawClock()
        {
            if (ClockCanvas == null) return;

            ClockCanvas.Children.Clear();
            var width = ClockCanvas.ActualWidth;
            var height = ClockCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            var size = Math.Min(width, height) - 40;
            var centerX = width / 2;
            var centerY = height / 2;

            bool isNight = ViewModel.IsLightOn;

            var dial = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = isNight ? new SolidColorBrush(Microsoft.UI.Colors.Black) : new SolidColorBrush(Microsoft.UI.Colors.White),
                Stroke = isNight ? new SolidColorBrush(Microsoft.UI.Colors.Black) : new SolidColorBrush(Microsoft.UI.Colors.White),
                StrokeThickness = 5
            };
            Canvas.SetLeft(dial, centerX - size / 2);
            Canvas.SetTop(dial, centerY - size / 2);
            ClockCanvas.Children.Add(dial);

            var numeralColor = isNight ? new SolidColorBrush(Microsoft.UI.Colors.LimeGreen) : new SolidColorBrush(Microsoft.UI.Colors.DarkSlateGray);
            var handColor = isNight ? new SolidColorBrush(Microsoft.UI.Colors.LimeGreen) : new SolidColorBrush(Microsoft.UI.Colors.DarkSlateGray);
            var secHandColor = isNight ? new SolidColorBrush(Microsoft.UI.Colors.Red) : new SolidColorBrush(Microsoft.UI.Colors.DarkRed);

            for (int i = 0; i < 12; i++)
            {
                var angle = Math.PI / 6 * i;
                var length = size / 2;
                var tickLength = size / 20;

                var x1 = centerX + Math.Sin(angle) * (length - tickLength);
                var y1 = centerY - Math.Cos(angle) * (length - tickLength);
                var x2 = centerX + Math.Sin(angle) * length;
                var y2 = centerY - Math.Cos(angle) * length;

                var tick = new Line
                {
                    X1 = x1, Y1 = y1, X2 = x2, Y2 = y2,
                    Stroke = numeralColor,
                    StrokeThickness = 3
                };
                ClockCanvas.Children.Add(tick);

                var num = i == 0 ? 12 : i;
                var numX = centerX + Math.Sin(angle) * (length - tickLength * 2);
                var numY = centerY - Math.Cos(angle) * (length - tickLength * 2);

                var text = new TextBlock
                {
                    Text = num.ToString(),
                    FontSize = size / 20,
                    Foreground = numeralColor,
                    FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                    TextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
                    Width = size / 10,
                    Height = size / 15
                };
                Canvas.SetLeft(text, numX - size / 20);
                Canvas.SetTop(text, numY - size / 30);
                ClockCanvas.Children.Add(text);
            }

            for (int i = 0; i < 60; i++)
            {
                if (i % 5 != 0)
                {
                    var angle = Math.PI / 30 * i;
                    var length = size / 2;
                    var tickLength = size / 40;

                    var x1 = centerX + Math.Sin(angle) * (length - tickLength);
                    var y1 = centerY - Math.Cos(angle) * (length - tickLength);
                    var x2 = centerX + Math.Sin(angle) * length;
                    var y2 = centerY - Math.Cos(angle) * length;

                    var tick = new Line
                    {
                        X1 = x1, Y1 = y1, X2 = x2, Y2 = y2,
                        Stroke = numeralColor,
                        StrokeThickness = 1
                    };
                    ClockCanvas.Children.Add(tick);
                }
            }

            var now = ViewModel.HighPrecisionTime;
            var hours = now.Hour % 12;
            var minutes = now.Minute;
            var seconds = now.Second + now.Millisecond / 1000.0;

            var hourAngle = Math.PI / 6 * (hours + minutes / 60.0 + seconds / 3600.0);
            var minuteAngle = Math.PI / 30 * (minutes + seconds / 60.0);
            var secondAngle = Math.PI / 30 * seconds;

            var hourLength = size / 3;
            var hx = centerX + Math.Sin(hourAngle) * hourLength;
            var hy = centerY - Math.Cos(hourAngle) * hourLength;
            var hourHand = new Line
            {
                X1 = centerX, Y1 = centerY, X2 = hx, Y2 = hy,
                Stroke = handColor,
                StrokeThickness = size / 30
            };
            ClockCanvas.Children.Add(hourHand);

            var minuteLength = size / 2 - 10;
            var mx = centerX + Math.Sin(minuteAngle) * minuteLength;
            var my = centerY - Math.Cos(minuteAngle) * minuteLength;
            var minuteHand = new Line
            {
                X1 = centerX, Y1 = centerY, X2 = mx, Y2 = my,
                Stroke = handColor,
                StrokeThickness = size / 50
            };
            ClockCanvas.Children.Add(minuteHand);

            var secondLength = size / 2 - 5;
            var sx = centerX + Math.Sin(secondAngle) * secondLength;
            var sy = centerY - Math.Cos(secondAngle) * secondLength;
            var secondHand = new Line
            {
                X1 = centerX, Y1 = centerY, X2 = sx, Y2 = sy,
                Stroke = secHandColor,
                StrokeThickness = size / 100
            };
            ClockCanvas.Children.Add(secondHand);

            var centerDot = new Ellipse
            {
                Width = size / 50,
                Height = size / 50,
                Fill = new SolidColorBrush(Microsoft.UI.Colors.Gray)
            };
            Canvas.SetLeft(centerDot, centerX - size / 100);
            Canvas.SetTop(centerDot, centerY - size / 100);
            ClockCanvas.Children.Add(centerDot);
        }

        private void OnSyncTimeClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.SyncSystemTime();
        }

        private async void OnAdjustTimeClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new TimeAdjustDialog(ViewModel.MasterTime)
            {
                XamlRoot = this.XamlRoot
            };
            
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ViewModel.AdjustTime(dialog.SelectedTime);
            }
        }

        private async void OnSettingsClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new SettingsDialog(ViewModel.LoadConfig())
            {
                XamlRoot = this.XamlRoot
            };
            
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ViewModel.SaveConfig(dialog.Config);
            }
        }
    }
}
