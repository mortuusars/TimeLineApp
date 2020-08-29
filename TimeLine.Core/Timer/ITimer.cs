using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLine.Core
{
    interface ITimer
    {
        public event EventHandler<int> TimerEnded;
        public event EventHandler<int> Countdown;

        public void Start(int timeForTimer);
        public void Add(int timeToAdd);
        public void Stop();
    }
}
