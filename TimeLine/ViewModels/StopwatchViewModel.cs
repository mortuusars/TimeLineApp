using System;
using System.ComponentModel;
using System.Windows.Input;

namespace TimeLine
{
    public class StopwatchViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Time { get; set; }

        public bool StopwatchRunning { get; set; }

        public ICommand StartPauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }


        private int stopwatch;
        private int Stopwatch
        {
            get { return stopwatch; }
            set { stopwatch = value; UpdateTimeString(); }
        }

        System.Timers.Timer counter;

        public StopwatchViewModel() {
            
            StartPauseCommand = new RelayCommand( act => { StartOrPause(); });
            StopCommand = new RelayCommand(act => { Stop(); });
            ResetCommand = new RelayCommand(act => { Reset(); });

            UpdateTimeString();

            counter = new System.Timers.Timer();
            counter.Interval = 1000;
            counter.Elapsed += Counter_Tick;
        }

        private void Counter_Tick(object sender, System.Timers.ElapsedEventArgs e) {
            Stopwatch++;
        }

        public void Start() {
            counter.Start();
            StopwatchRunning = true;
        }

        public void Pause() {
            counter.Stop();
            StopwatchRunning = false;
        }

        public void Stop() {
            Pause();
            Reset();
        }

        public void Reset() {
            Stopwatch = 0;
        }



        private void StartOrPause() {
            if (counter.Enabled)
                Pause();
            else 
                Start();
        }



        private void UpdateTimeString() {
            Time = TimeSpan.FromSeconds(Stopwatch).ToString();;
        }
    }
}
