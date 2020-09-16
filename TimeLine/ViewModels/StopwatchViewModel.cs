using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace TimeLine
{
    public class StopwatchViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        StopwatchManager StopwatchManager;
        public StopwatchCounter Stopwatch { get; set; }

        private double left;
        public double Left
        {
            get { return left; }
            set { left = value; SaveWindowPositionLeft(); }
        }

        private double top;
        public double Top
        {
            get { return top; }
            set { top = value; SaveWindowPositionTop(); }
        }


        public double Width { get; set; } = 326;

        public string Time { get; set; } = "";
        public string GhostTime { get; set; } = "00:00:00";

        public bool DayOne { get; set; }
        public bool DayTwo { get; set; }

        public bool WindowClosing { get; set; }

        public ICommand StartPauseCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        public StopwatchViewModel(StopwatchManager stopwatchManager) {            
            StopwatchManager = stopwatchManager;

            StartPauseCommand = new RelayCommand( act => Stopwatch.StartPause());
            StartCommand = new RelayCommand(act => Stopwatch.Start(), canEx => !IsStoppable());
            PauseCommand = new RelayCommand(act => Stopwatch.Pause(), canEx => IsStoppable());
            StopCommand = new RelayCommand(act => Stopwatch.Stop(), canEx => IsStoppable());
            ResetCommand = new RelayCommand(act =>  Stopwatch.Reset());
            CloseCommand = new RelayCommand(act => StopwatchManager.Close());

            SetWindowPosition();

            Stopwatch = new StopwatchCounter();
            Stopwatch.CountChanged += (s, count) => { SetTimeString(count); SetDayIsVisible(count); };
        }




        private void SetWindowPosition() {
            var left = App.ApplicationSettings.AppSettings.StopwatchPositionLeft;

            if (left == default)
                left = WpfScreenHelper.Screen.PrimaryScreen.Bounds.Width / 2 - Width / 2;

            Left = left;

            var top = App.ApplicationSettings.AppSettings.StopwatchPositionTop;

            if (top == default)
                top = WpfScreenHelper.Screen.PrimaryScreen.Bounds.Height / 2;

            Top = top;
        }

        private void SaveWindowPositionLeft() {
            App.ApplicationSettings.AppSettings.StopwatchPositionLeft = Left;
        }

        private void SaveWindowPositionTop() {
            App.ApplicationSettings.AppSettings.StopwatchPositionTop = Top;

        }




        /// <summary>
        /// Returns true if stopwatch is running and can be stopped.
        /// </summary>
        /// <returns></returns>
        public bool IsStoppable() {
            return Stopwatch.StopwatchRunning;
        }


        /// <summary>
        /// Sets Time and GhostTime to proper values based on elapsed seconds.
        /// </summary>
        /// <param name="seconds"></param>
        private void SetTimeString(int seconds) {            
            string newTime = TimeSpan.FromSeconds(seconds).ToString();
            if (newTime.Length > 8) {
                for (int i = newTime.Length; i > 8; i--) {
                    newTime = newTime.Remove(0, 1);
                }
            }
            
            string newGhostTime = "";
            string spacesToReplace = "";

            foreach (var ch in newTime) {
                if (ch == '0' || ch == ':') {
                    newGhostTime += ch;
                    spacesToReplace += " ";
                }
                else
                    break;
            }

            GhostTime = newGhostTime;
            
            if (newGhostTime.Length == 1) {
                var sb = new StringBuilder(newTime);
                sb[0] = ' ';
                Time = sb.ToString();                
            }
            else if (newGhostTime.Length > 1)
                Time = newTime.Replace(newGhostTime, spacesToReplace);
            else
                Time = newTime;
        }

        private void SetDayIsVisible(int count) {
            // One day
            if (count > 172_800) {
                DayOne = true;
                DayTwo = true;
            }
            else if (count > 86_400) {
                DayOne = true;
                DayTwo = false;
            }
            else {
                DayOne = false;
                DayTwo = false;
            }
                
        }
    }
}
