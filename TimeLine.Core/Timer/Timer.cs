using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLine.Core
{
    public class Timer : ITimer
    {
        public event EventHandler<int> TimerEnded;
        public event EventHandler<int> Countdown;

        public int RemainingSeconds
        {
            get { return remainingSeconds; }
            set {
                remainingSeconds = value;
                Countdown?.Invoke(this, RemainingSeconds);
            }
        }
        public bool IsRunning { get; set; }

        long startTime;
        long currentTime;
        long timerTime;

        private int remainingSeconds;

        System.Timers.Timer timer;

        public void Start(int time) {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;

            currentTime = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
            startTime = currentTime;
            timerTime = currentTime + time;

            RemainingSeconds = time;

            timer.Start();
            IsRunning = true;
        }

        public void Add(int timeToAdd) {
            timerTime += timeToAdd;
        }

        public void Stop() {
            timer.Stop();
            timer.Dispose();

            RemainingSeconds = 0;
            IsRunning = false;
        }



        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if (currentTime >= timerTime) {
                TimerEnded?.Invoke(this, (int)(timerTime - startTime));
                Stop();
            }
            else {
                RemainingSeconds = (int)(timerTime - currentTime);
                currentTime = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
            }
        }
    }
}
