using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using TimeLine.ViewModels;

namespace TimeLine.Models
{
    public class ToastControlViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Icon { get; set; }

        public ICommand CloseCommand { get; private set; }

        public ToastControlViewModel(string title, string message, Icons icon) {
            Title = title;
            Message = message;
            Icon = GetIcon(icon);

            CloseCommand = new RelayCommand( act => { GetService.Manager.CloseToast(this, fromCommand : true); });
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
