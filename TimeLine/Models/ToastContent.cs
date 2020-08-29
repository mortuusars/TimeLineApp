using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using TimeLine.ViewModels;

namespace TimeLine.Models
{
    public class ToastContent
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Icon { get; set; }

        public ICommand CloseCommand { get; private set; }

        public ToastContent(string title, string message, IconType icon) {
            Title = title;
            Message = message;
            Icon = GetIcon(icon);

            CloseCommand = new RelayCommand( act => { GetService.Manager.CloseToast(this); });
        }

        private string GetIcon(IconType icon) {
            if (icon == IconType.alarm)
                return "../Resources/Icons/alarm_white_64.png";
            else if (icon == IconType.clock)
                return "../Resources/Icons/clock_white_64.png";
            else if (icon == IconType.hourglass)
                return "../Resources/Icons/hourglass_white_64.png";
            else if (icon == IconType.stopwatch)
                return "../Resources/Icons/stopwatch_white_64.png";
            else if (icon == IconType.timer)
                return "../Resources/Icons/timer_white_64.png";
            else
                return "../Resources/Icons/info_white_64.png";
        }
    }
}
