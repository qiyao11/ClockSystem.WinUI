using Microsoft.UI.Xaml.Controls;
using System;

namespace ClockSystem.WinUI.Dialogs
{
    public sealed partial class TimeAdjustDialog : ContentDialog
    {
        public DateTime SelectedTime { get; private set; }

        public TimeAdjustDialog(DateTime currentTime)
        {
            this.InitializeComponent();

            YearBox.Value = currentTime.Year;
            MonthBox.Value = currentTime.Month;
            DayBox.Value = currentTime.Day;
            HourBox.Value = currentTime.Hour;
            MinuteBox.Value = currentTime.Minute;
            SecondBox.Value = currentTime.Second;
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
    }
}
