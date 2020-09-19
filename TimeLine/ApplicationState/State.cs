using System.Collections.Generic;
using TimeLine.Core;

namespace TimeLine
{
    public class State
    {
        public long TimerRingTime { get; set; }
        public int StopwatchCount { get; set; }
        public bool StopwatchRunning { get; set; }
        public bool StopwatchWindowOpen { get; set; }
        public List<Alarm> Alarms { get; set; }
    }
}
