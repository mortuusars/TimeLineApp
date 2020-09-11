using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
using TimeLine.Views;

namespace TimeLine
{
    public class StopwatchManager
    {
        StopwatchView StopwatchView;
        StopwatchViewModel StopwatchVM;

        public bool IsRunning { get { return StopwatchVM != null ? StopwatchVM.IsStoppable() : false; } }

        public void OpenCloseWindow() {
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
                OpenCloseWindow();

            StopwatchVM.StartPauseCommand.Execute(null);
        }
        
        public bool Start() {
            if (CheckIfNullView())
                OpenCloseWindow();

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
            if (StopwatchVM != null) {
                StopwatchVM.WindowClosing = true;
                
                var timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
                timer.Tick += (s, e) => { CloseWindow(); timer.Stop(); };
                timer.Start();
            }            
        }

        private void CloseWindow() {
            if (CheckIfNullView() == false) {
                StopwatchView.Close();
                StopwatchView = null;
                StopwatchVM = null;
            }
        }

        /// <summary>
        /// Returns true if StopwatchView is null.
        /// </summary>
        private bool CheckIfNullView() {
            return StopwatchView == null ? true : false;
        }

    }
}
