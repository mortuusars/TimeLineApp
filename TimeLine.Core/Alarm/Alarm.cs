using System;

namespace TimeLine.Core
{
    public class Alarm
    {
        public DateTimeOffset RingTime { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public bool ShouldRing { get; set; }

        private bool hasRangRecently;

        public bool HasRangRecently
        {
            get { return hasRangRecently; }
            set {
                if (value == true) {
                    RingAgainAfterDelay();
                }
                hasRangRecently = value;
            }
        }


        public Alarm(DateTimeOffset ringTime) {
            RingTime = ringTime;
            CreationTime = DateTimeOffset.Now;
            ShouldRing = true;
        }


        private void RingAgainAfterDelay() {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000;
            timer.Elapsed += (s, e) => { HasRangRecently = false; timer.Stop(); };
            timer.Start();
        }
    }
}
