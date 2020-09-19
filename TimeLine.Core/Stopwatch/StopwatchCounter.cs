using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TimeLine
{
    public class StopwatchCounter
    {
        /// <summary>
        /// Raises every time stopwatch count changes.
        /// </summary>
        public event EventHandler CountChanged;

        /// <summary>
        /// Returns true if Stopwatch is counting.
        /// </summary>
        public bool StopwatchRunning { get; set; }

        public int Count
        {
            get { return count; }
            set { 
                count = value; 
                CountChanged?.Invoke(this, EventArgs.Empty); 
            }
        }        

        System.Timers.Timer stopwatchCounter;
        private int count;

        public StopwatchCounter() {
            stopwatchCounter = new System.Timers.Timer();
            stopwatchCounter.Interval = 1000;
            stopwatchCounter.Elapsed += Counter_Tick;
        }


        #region Public Methods

        public void Start() {
            stopwatchCounter.Start();
            StopwatchRunning = true;
        }  

        public void Stop() {
            stopwatchCounter.Stop();
            StopwatchRunning = false;
        }

        #endregion


        private void Counter_Tick(object sender, System.Timers.ElapsedEventArgs e) {
            Count++;
        }
    }
}
