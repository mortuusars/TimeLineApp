using System.Collections.Generic;
using TimeLine.Core;

namespace TimeLine
{
    public class State
    {
        public int TimerRingTime { get; set; }
        public int StopwatchCount { get; set; }
        public List<Alarm> Alarms { get; set; }
    }
}
