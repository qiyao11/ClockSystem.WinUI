using ClockSystem.WinUI.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO;

namespace ClockSystem.WinUI.Dialogs
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        public ConfigModel Config { get; private set; }

        public SettingsDialog(ConfigModel config)
        {
            this.InitializeComponent();
            Config = config;
            DataContext = this;
            
            PopulatePathItems();
            PopulateSwitchItems();
        }

        private void PopulatePathItems()
        {
            HourPathsItemsControl.Items.Clear();
            foreach (var item in Config.Path.HourPaths)
            {
                var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 2), Spacing = 5 };
                panel.Children.Add(new TextBlock { Text = item.Key.ToString(), Width = 40, VerticalAlignment = VerticalAlignment.Center });
                panel.Children.Add(new TextBlock { Text = "点:", VerticalAlignment = VerticalAlignment.Center });
                var textBox = new TextBox { Text = item.Value, Width = 200 };
                textBox.TextChanged += (s, e) =>
                {
                    var tb = s as TextBox;
                    for (int i = 0; i < Config.Path.HourPaths.Count; i++)
                    {
                        if (Config.Path.HourPaths[i].Key == item.Key)
                        {
                            Config.Path.HourPaths[i] = new Models.KeyValuePair<int, string>(item.Key, tb.Text);
                            break;
                        }
                    }
                };
                panel.Children.Add(textBox);
                HourPathsItemsControl.Items.Add(panel);
            }

            HalfPathsItemsControl.Items.Clear();
            foreach (var item in Config.Path.HalfPaths)
            {
                var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 2), Spacing = 5 };
                panel.Children.Add(new TextBlock { Text = item.Key.ToString(), Width = 40, VerticalAlignment = VerticalAlignment.Center });
                panel.Children.Add(new TextBlock { Text = "点半:", VerticalAlignment = VerticalAlignment.Center });
                var textBox = new TextBox { Text = item.Value, Width = 200 };
                textBox.TextChanged += (s, e) =>
                {
                    var tb = s as TextBox;
                    for (int i = 0; i < Config.Path.HalfPaths.Count; i++)
                    {
                        if (Config.Path.HalfPaths[i].Key == item.Key)
                        {
                            Config.Path.HalfPaths[i] = new Models.KeyValuePair<int, string>(item.Key, tb.Text);
                            break;
                        }
                    }
                };
                panel.Children.Add(textBox);
                HalfPathsItemsControl.Items.Add(panel);
            }
        }

        private void PopulateSwitchItems()
        {
            HourSwitchesItemsControl.Items.Clear();
            foreach (var item in Config.Switch.HourSwitches)
            {
                var checkBox = new CheckBox { Content = $"{item.Key}点", IsChecked = item.Value, Margin = new Thickness(5) };
                checkBox.Checked += (s, e) =>
                {
                    for (int i = 0; i < Config.Switch.HourSwitches.Count; i++)
                    {
                        if (Config.Switch.HourSwitches[i].Key == item.Key)
                        {
                            Config.Switch.HourSwitches[i] = new Models.KeyValuePair<int, bool>(item.Key, true);
                            break;
                        }
                    }
                };
                checkBox.Unchecked += (s, e) =>
                {
                    for (int i = 0; i < Config.Switch.HourSwitches.Count; i++)
                    {
                        if (Config.Switch.HourSwitches[i].Key == item.Key)
                        {
                            Config.Switch.HourSwitches[i] = new Models.KeyValuePair<int, bool>(item.Key, false);
                            break;
                        }
                    }
                };
                HourSwitchesItemsControl.Items.Add(checkBox);
            }

            HalfSwitchesItemsControl.Items.Clear();
            foreach (var item in Config.Switch.HalfSwitches)
            {
                var checkBox = new CheckBox { Content = $"{item.Key}点半", IsChecked = item.Value, Margin = new Thickness(5) };
                checkBox.Checked += (s, e) =>
                {
                    for (int i = 0; i < Config.Switch.HalfSwitches.Count; i++)
                    {
                        if (Config.Switch.HalfSwitches[i].Key == item.Key)
                        {
                            Config.Switch.HalfSwitches[i] = new Models.KeyValuePair<int, bool>(item.Key, true);
                            break;
                        }
                    }
                };
                checkBox.Unchecked += (s, e) =>
                {
                    for (int i = 0; i < Config.Switch.HalfSwitches.Count; i++)
                    {
                        if (Config.Switch.HalfSwitches[i].Key == item.Key)
                        {
                            Config.Switch.HalfSwitches[i] = new Models.KeyValuePair<int, bool>(item.Key, false);
                            break;
                        }
                    }
                };
                HalfSwitchesItemsControl.Items.Add(checkBox);
            }
        }

        private void BrowseMusicButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.MusicLibrary
            };
            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".wav");
            picker.FileTypeFilter.Add(".ogg");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = picker.PickSingleFileAsync().AsTask().Result;
            if (file != null)
            {
                Config.Path.MusicPath = file.Path;
            }
        }

        private void BrowseBellButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.MusicLibrary
            };
            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".wav");
            picker.FileTypeFilter.Add(".ogg");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = picker.PickSingleFileAsync().AsTask().Result;
            if (file != null)
            {
                Config.Path.BellPath = file.Path;
            }
        }

        private void SelectAllHourButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Config.Switch.HourSwitches.Count; i++)
            {
                var hour = Config.Switch.HourSwitches[i].Key;
                Config.Switch.HourSwitches[i] = new Models.KeyValuePair<int, bool>(hour, true);
            }
            PopulateSwitchItems();
        }

        private void DeselectAllHourButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Config.Switch.HourSwitches.Count; i++)
            {
                var hour = Config.Switch.HourSwitches[i].Key;
                Config.Switch.HourSwitches[i] = new Models.KeyValuePair<int, bool>(hour, false);
            }
            PopulateSwitchItems();
        }

        private void SelectAllHalfButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Config.Switch.HalfSwitches.Count; i++)
            {
                var hour = Config.Switch.HalfSwitches[i].Key;
                Config.Switch.HalfSwitches[i] = new Models.KeyValuePair<int, bool>(hour, true);
            }
            PopulateSwitchItems();
        }

        private void DeselectAllHalfButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Config.Switch.HalfSwitches.Count; i++)
            {
                var hour = Config.Switch.HalfSwitches[i].Key;
                Config.Switch.HalfSwitches[i] = new Models.KeyValuePair<int, bool>(hour, false);
            }
            PopulateSwitchItems();
        }
    }
}
