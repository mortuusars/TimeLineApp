using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace TimeLine.ViewModels
{
    public class ToastViewModel
    {
        public double Left { get; set; }
        public int Top { get; set; }

        public double RightBorder { get; set; }

        public int Width { get; set; } = 350;
        public int Height { get; set; } = 150;

        public string Icon { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public ICommand CloseCommand { get; set; }

        public ToastViewModel(string title, string message, Icons iconType = Icons.info) {
            Title = title;
            Message = message;
            Icon = GetIcon(iconType);

            Left = WpfScreenHelper.Screen.PrimaryScreen.Bounds.Right - Width * 1.05;
            Top = (int)(WpfScreenHelper.Screen.PrimaryScreen.Bounds.Bottom - Height * 1.1);
            RightBorder = WpfScreenHelper.Screen.PrimaryScreen.Bounds.Right;

            //CloseCommand = new RelayCommand(act => { GetService.Manager.CloseToast(); });
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
