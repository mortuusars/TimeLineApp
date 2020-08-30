using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TimeLine.ViewModels;

namespace TimeLine.Models
{
    public class ToastControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title { get; set; }
        public string Message { get; set; }
        public string Icon { get; set; }
        public SolidColorBrush IconTintColor { get; set; }

        //TODO: Postpone button?
        public bool PostponeButtonIsVisible { get; set; } = false;

        public bool IsRemoving { get; set; }

        public ICommand CloseCommand { get; private set; }
        public object FindResource { get; private set; }

        public ToastControlViewModel(string title, string message, Icons icon) {
            Title = title;
            Message = message;
            Icon = GetIcon(icon);
            IconTintColor = GetTintColor(icon);

            CloseCommand = new RelayCommand( act => { GetService.Manager.CloseToast(this, fromCommand : true); });
        }

        

        public void Close() {
            IsRemoving = true;

            var FadeDuration = (Duration)App.Current.FindResource("ToastFadeOutDuration");

            var timer = new DispatcherTimer();
            timer.Interval = FadeDuration.TimeSpan;
            timer.Tick += (sender, e) => 
            { 
                GetService.Manager.RemoveToastFromList(this); 
                timer.Stop();
            };
            timer.Start();
        }


        private string GetIcon(Icons icon) {
            if (icon == Icons.alarm)
                return "pack://application:,,,/Resources/Icons/alarm_white_64.png";
            else if (icon == Icons.clock)
                return "pack://application:,,,/Resources/Icons/clock_white_64.png";
            else if (icon == Icons.hourglass)
                return "pack://application:,,,/Resources/Icons/hourglass_white_64.png";
            else if (icon == Icons.stopwatch)
                return "pack://application:,,,/Resources/Icons/stopwatch_white_64.png";
            else if (icon == Icons.timer)
                return "pack://application:,,,/Resources/Icons/timer_white_64.png";
            else
                return "pack://application:,,,/Resources/Icons/info_white_64.png";
        }

        private SolidColorBrush GetTintColor(Icons icon) {
            switch (icon) {
                case Icons.timer:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#b8e58a");  // Green
                case Icons.stopwatch:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#a1c8e5");  // Light Blue
                case Icons.alarm:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#e5c4a1");  // Orange
                case Icons.clock:
                case Icons.hourglass:
                case Icons.info:
                default:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFF");  // White
            }
        }
    }
}
