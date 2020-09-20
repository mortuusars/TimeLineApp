using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TimeLine.Core
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
