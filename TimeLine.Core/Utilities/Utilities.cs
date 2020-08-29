using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLine.Core
{
    public class Utilities
    {
        public static string IntToPrettyTime(int inputSeconds) {
            var timeSpan = TimeSpan.FromSeconds(inputSeconds);

            string minutes = timeSpan.Minutes < 10 ? $"0{timeSpan.Minutes}" : timeSpan.Minutes.ToString();
            string seconds = timeSpan.Seconds < 10 ? $"0{timeSpan.Seconds}" : timeSpan.Seconds.ToString();

            if (timeSpan.Hours != 0)
                if (timeSpan.Minutes == 0 && timeSpan.Seconds == 0)
                    return $"{timeSpan.Hours} hours";
                else if (timeSpan.Minutes == 0 && timeSpan.Seconds != 0)
                    return $"{timeSpan.Hours} hours and {seconds} seconds";
                else
                    return $"{timeSpan.Hours}:{minutes}:{seconds}";
            else if (timeSpan.Minutes != 0)
                if (timeSpan.Seconds == 0)
                    return $"{timeSpan.Minutes} minutes";
                else
                    return $"{timeSpan.Minutes}:{seconds}";
            else
                return $"{timeSpan.Seconds} seconds";
        }
    }
}
