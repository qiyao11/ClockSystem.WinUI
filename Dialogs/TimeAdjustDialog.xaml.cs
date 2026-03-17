using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace ClockSystem.WinUI.Dialogs
{
    public sealed partial class TimeAdjustDialog : ContentDialog
    {
        public DateTime SelectedTime { get; private set; }
        private DateTime _originalTime;

        public TimeAdjustDialog(DateTime currentTime)
        {
            this.InitializeComponent();
            _originalTime = currentTime;

            YearBox.Value = currentTime.Year;
            MonthBox.Value = currentTime.Month;
            DayBox.Value = currentTime.Day;
            HourBox.Value = currentTime.Hour;
            MinuteBox.Value = currentTime.Minute;
            SecondBox.Value = currentTime.Second;

            // Attach value changed handlers for preview update
            YearBox.ValueChanged += (s, e) => UpdatePreview();
            MonthBox.ValueChanged += (s, e) => UpdatePreview();
            DayBox.ValueChanged += (s, e) => UpdatePreview();
            HourBox.ValueChanged += (s, e) => UpdatePreview();
            MinuteBox.ValueChanged += (s, e) => UpdatePreview();
            SecondBox.ValueChanged += (s, e) => UpdatePreview();

            UpdatePreview();
        }

        private void UpdatePreview()
        {
            try
            {
                var year = (int)YearBox.Value;
                var month = (int)MonthBox.Value;
                var day = (int)DayBox.Value;
                var hour = (int)HourBox.Value;
                var minute = (int)MinuteBox.Value;
                var second = (int)SecondBox.Value;
                
                var previewTime = new DateTime(year, month, day, hour, minute, second);
                PreviewTextBlock.Text = previewTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                PreviewTextBlock.Text = "无效日期时间";
            }
        }

        private void OnApplyClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                SelectedTime = new DateTime(
                    (int)YearBox.Value,
                    (int)MonthBox.Value,
                    (int)DayBox.Value,
                    (int)HourBox.Value,
                    (int)MinuteBox.Value,
                    (int)SecondBox.Value
                );
            }
            catch (Exception ex)
            {
                args.Cancel = true;
                var errorDialog = new ContentDialog
                {
                    Title = "错误",
                    Content = "时间设置错误: " + ex.Message,
                    CloseButtonText = "确定",
                    XamlRoot = this.XamlRoot
                };
                _ = errorDialog.ShowAsync();
            }
        }

        private void SetCurrentTimeButton_Click(object sender, RoutedEventArgs e)
        {
            var now = DateTime.Now;
            YearBox.Value = now.Year;
            MonthBox.Value = now.Month;
            DayBox.Value = now.Day;
            HourBox.Value = now.Hour;
            MinuteBox.Value = now.Minute;
            SecondBox.Value = now.Second;
            UpdatePreview();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            YearBox.Value = _originalTime.Year;
            MonthBox.Value = _originalTime.Month;
            DayBox.Value = _originalTime.Day;
            HourBox.Value = _originalTime.Hour;
            MinuteBox.Value = _originalTime.Minute;
            SecondBox.Value = _originalTime.Second;
            UpdatePreview();
        }
    }
}
