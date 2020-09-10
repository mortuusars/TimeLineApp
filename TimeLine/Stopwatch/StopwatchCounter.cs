using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TimeLine
{
    public class StopwatchCounter : INotifyPropertyChanged
    {
        public event EventHandler<int> StopwatchTick;
        public event PropertyChangedEventHandler PropertyChanged;

        System.Timers.Timer counter;

        public bool StopwatchRunning { get; set; }

        private int stopwatchCount;
        private int StopwatchCount
        {
            get { return stopwatchCount; }
            set { stopwatchCount = value; StopwatchTick?.Invoke(this, StopwatchCount); }
        }

        public StopwatchCounter() {
            counter = new System.Timers.Timer();
            counter.Interval = 1000;
            counter.Elapsed += Counter_Tick;
        }

        private void Counter_Tick(object sender, System.Timers.ElapsedEventArgs e) {
            StopwatchCount++;
        }

        /// <summary>
        /// Starts or (if running) pauses stopwatch.
        /// </summary>
        public void StartPause() {
            if (counter.Enabled == false) 
                Start();
            else 
                Pause();
        }

        /// <summary>
        /// Stops and resets stopwatch count.
        /// </summary>
        public void Stop() {
            Pause();
            Reset();
        }

        public void Reset() {
            StopwatchCount = 0;
        }


        public void Start() {
            counter.Start();
            StopwatchRunning = true;
        }

        public void Pause() {
            counter.Stop();
            StopwatchRunning = false;
        }
    }
}
