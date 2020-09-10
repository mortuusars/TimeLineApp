using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace TimeLine
{
    public class StopwatchViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public StopwatchCounter Stopwatch { get; set; }

        public string Time { get; set; }
        public string GhostTime { get; set; }

        public ICommand StartPauseCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }

        public StopwatchViewModel() {
            
            StartPauseCommand = new RelayCommand( act => Stopwatch.StartPause());
            StartCommand = new RelayCommand(act => Stopwatch.Start(), canEx => !IsStoppable());
            PauseCommand = new RelayCommand(act => Stopwatch.Pause(), canEx => IsStoppable());
            StopCommand = new RelayCommand(act => Stopwatch.Stop(), canEx => IsStoppable());
            ResetCommand = new RelayCommand(act =>  Stopwatch.Reset());

            Stopwatch = new StopwatchCounter();
            Stopwatch.StopwatchTick += StopwatchTick;

            UpdateTimeString(0);
        }

        private void StopwatchTick(object sender, int stopwatchCounter) {
            UpdateTimeString(stopwatchCounter);
        }

        private void UpdateTimeString(int time) {            
            string newTime = TimeSpan.FromSeconds(time).ToString();
            
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

        public bool IsStoppable() {
            return Stopwatch.StopwatchRunning;
        }
    }
}
