using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
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

        //TODO: Postpone button?
        public bool PostponeButtonIsVisible { get; set; } = false;

        public bool IsRemoving { get; set; }

        public ICommand CloseCommand { get; private set; }
        public object FindResource { get; private set; }

        public ToastControlViewModel(string title, string message, Icons icon) {
            Title = title;
            Message = message;
            Icon = GetIcon(icon);

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
            };
            timer.Start();
        }


        private string GetIcon(Icons icon) {
            if (icon == Icons.alarm)
                return "../Resources/Icons/alarm_white_64.png";
            else if (icon == Icons.clock)
                return "../Resources/Icons/clock_white_64.png";
            else if (icon == Icons.hourglass)
                return "../Resources/Icons/hourglass_white_64.png";
            else if (icon == Icons.stopwatch)
                return "../Resources/Icons/stopwatch_white_64.png";
            else if (icon == Icons.timer)
                return "../Resources/Icons/timer_white_64.png";
            else
                return "../Resources/Icons/info_white_64.png";
        }
    }
}
