using System.Collections.Generic;
using TimeLine.Core;

namespace TimeLine
{
    public class State
    {
        public long TimerRingTime { get; set; }
        public int StopwatchCount { get; set; }
        public List<Alarm> Alarms { get; set; }
    }
}
