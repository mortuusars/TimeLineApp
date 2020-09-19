using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;
using TimeLine.Views;

namespace TimeLine
{
    public class StopwatchManager : INotifyPropertyChanged
    {
        private StopwatchView StopwatchView;
        private StopwatchCounter StopwatchCounter;

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

        public bool OneDay { get; set; }
        public bool TwoDays { get; set; }

        public bool Closing { get; set; }

        public ICommand StartPauseCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }



        public StopwatchManager() {
            StartPauseCommand = new RelayCommand(act => StartOrPause());
            StartCommand = new RelayCommand(act => Start(), canEx => !IsRunning());
            PauseCommand = new RelayCommand(act => Pause(), canEx => IsRunning());
            StopCommand = new RelayCommand(act => Stop(), canEx => IsRunning());
            ResetCommand = new RelayCommand(act => Reset());
            CloseCommand = new RelayCommand(act => Close());


            StopwatchCounter = new StopwatchCounter();
            StopwatchCounter.CountChanged += (s, e) => { UpdateTimeString(StopwatchCounter.Count); };
        }



        public void ToggleWindow() {
            if (StopwatchView == null)
                Open();
            else
                Close();
        }

        public void Open() {
            Close();

            Closing = false;

            SetWindowPosition();

            StopwatchView = new StopwatchView() { DataContext = this };
            StopwatchView.Show();
        }

        public void Close() {
            Closing = true;
            Reset();
            StopwatchView?.Close();
            StopwatchView = null;
        }

        public void Start() {
            if (StopwatchView == null)
                Open();

            StopwatchCounter.Start();
        }

        public void Start(int startFrom) {
            if (startFrom < 0)
                throw new ArgumentOutOfRangeException(nameof(startFrom));

            Start();

            StopwatchCounter.Count = startFrom;
        }

        public void StartOrPause() {
            if (StopwatchCounter.StopwatchRunning)
                Pause();
            else
                Start();
        }

        public void Pause() {
            StopwatchCounter.Stop();
        }

        public void Reset() {
            StopwatchCounter.Count = 0;
        }

        public bool Stop() {
            if (StopwatchCounter.StopwatchRunning) {
                Pause();
                Reset();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Returns true if stopwatch is running and can be stopped.
        /// </summary>
        public bool IsRunning() => StopwatchCounter.StopwatchRunning;

        public int GetCount() => StopwatchCounter.Count;

        public bool IsWindowOpen() => StopwatchView == null ? false : true;



        /// <summary>
        /// Sets Time and GhostTime to proper values based on elapsed seconds.
        /// </summary>
        /// <param name="seconds"></param>
        private void UpdateTimeString(int seconds) {
            string newTime = TimeSpan.FromSeconds(seconds).ToString();
            if (newTime.Length > 8) {  
               OneDay = true;
                
                for (int i = newTime.Length; i > 8; i--) {
                    newTime = newTime.Remove(0, 1);
                }
            }
            else 
                OneDay = false;

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

        /*
        private void SetDaysVisibilityTrigger(int count) {
            if (count > 172_800) {
                OneDay = true;
                TwoDays = true;
            }
            else if (count > 86_400) {
                OneDay = true;
                TwoDays = false;
            }
            else {
                OneDay = false;
                TwoDays = false;
            }
        }
        */


        #region Position

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

        #endregion




        /*
        StopwatchView StopwatchView;
        StopwatchViewModel StopwatchVM;

        public bool IsRunning { get { return StopwatchVM != null ? StopwatchVM.IsStoppable() : false; } }
        public int GetCount { get { return StopwatchVM == null ? 0 : StopwatchVM.Count; }}

        public void OpenOrCloseWindow() {
            if (StopwatchView == null) {

                StopwatchView = new StopwatchView();
                StopwatchVM = new StopwatchViewModel(this);

                StopwatchView.DataContext = StopwatchVM;
                StopwatchView.Show();
            }
            else {
                Close();
            }
        }

        public void StartPause() {
            if (CheckIfNullView())
                OpenOrCloseWindow();

            StopwatchVM.StartPauseCommand.Execute(null);
        }
        
        public bool Start() {
            if (CheckIfNullView())
                OpenOrCloseWindow();

            StopwatchVM.StartCommand.Execute(null);
            return true;
        }

        /// <summary>
        /// Pauses stopwatch if it is running.
        /// </summary>
        /// <returns></returns>
        public bool Pause() {
            if (CheckIfNullView())
                return false;
            else {
                StopwatchVM.PauseCommand.Execute(null);
                return true;
            }
        }

        public void Reset() {
            if (CheckIfNullView() == false)
                StopwatchVM.ResetCommand.Execute(null);
        }

        /// <summary>
        /// Stops the stopwatch. Returns true if stopped successfully.
        /// </summary>
        /// <returns></returns>
        public bool Stop() {
            if (CheckIfNullView())
                return false;
            else {
                if (StopwatchVM.StopCommand.CanExecute(null)) {
                    StopwatchVM.StopCommand.Execute(null);
                    return true;
                }
                else
                    return false;
            }
        }

        public void Close() {
            StopwatchVM?.Close();
            StopwatchView?.Close();

            StopwatchView = null;
            StopwatchVM = null;
        }

        /// <summary>
        /// Returns true if StopwatchView is null.
        /// </summary>
        private bool CheckIfNullView() {
            return StopwatchView == null ? true : false;
        }

        */


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
