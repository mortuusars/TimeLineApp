using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLine
{
    public class AppSettings
    {
        public string SoundFilePath { get; set; }
        public int SoundVolume { get; set; }

        public int ToastNotificationDelayInSeconds { get; set; }

        public double StopwatchPositionLeft { get; set; }
        public double StopwatchPositionTop { get; set; }
    }
}
