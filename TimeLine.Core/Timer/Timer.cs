using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLine.Core
{
    public class Timer : ITimer
    {
        public event EventHandler<int> TimerEnded;
        public event EventHandler<int> Countdown;

        public bool IsRunning { get; set; }

        int startTime;
        int currentTime;
        int timerTime;

        System.Timers.Timer timer;

        public void Start(int time) {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;

            currentTime = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
            startTime = currentTime;
            timerTime = currentTime + time;

            timer.Start();
            IsRunning = true;
        }

        public void Add(int timeToAdd) {
            timerTime += timeToAdd;
        }

        public void Stop() {
            timer.Stop();
            timer.Dispose();

            IsRunning = false;
            // Pass 0, as timer is not running
            Countdown?.Invoke(this, 0);
        }



        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if (currentTime >= timerTime) {
                TimerEnded?.Invoke(this, timerTime - startTime);
                Stop();
            }
            else {
                Countdown?.Invoke(this, timerTime - currentTime);
                currentTime = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
            }
        }
    }
}
