using System;

namespace TimeLine.Core
{
    public class Utilities
    {
        public static string PrettyTime(int inputSeconds, bool removeMinusSign = false) {
            var span = TimeSpan.FromSeconds(inputSeconds);

            string minutesFull = span.Minutes < 10 ? $"0{span.Minutes}" : span.Minutes.ToString();
            string secondsFull = span.Seconds < 10 ? $"0{span.Seconds}" : span.Seconds.ToString();



            // Hours
            string hrs = span.Hours > 1 ? $"{span.Hours} hours" : $"{span.Hours} hour";                   // Singular or plural;

            // Minutes
            string mins = "";

            if (span.Hours != 0)
                mins = minutesFull;
            else
                mins = span.Minutes.ToString();

            mins = span.Minutes > 1 ? $"{mins} minutes" : $"{mins} minute";

            // Seconds
            string secs = "";

            if (span.Minutes != 0)
                secs = secondsFull;
            else
                secs = span.Seconds.ToString();

            secs = span.Seconds > 1 ? $"{secs} seconds" : $"{secs} second";


            string prettyTime = "";

            // Has hours
            if (span.Hours != 0) {

                if (span.Minutes == 0 && span.Seconds == 0)                                             // Has only hours
                    prettyTime = hrs;

                else if (span.Minutes == 0 && span.Seconds != 0)                                        // Has seconds but no minutes
                    prettyTime = $"{hrs} and {secs}";

                else if (span.Seconds == 0)
                    prettyTime = $"{hrs} and {mins}";

                else
                    prettyTime = $"{hrs} {mins} {secs}";
            }
            // Has Minutes
            else if (span.Minutes != 0) // Has Minutes

                if (span.Seconds == 0)
                    prettyTime = mins;
                else
                    prettyTime = $"{mins} and {secs}";
            // Has only Seconds
            else
                prettyTime = secs;


            // Remove minus sign from string, if needed
            return removeMinusSign ? prettyTime.Replace('-', '\0') : prettyTime;
        }

        public static string TimeToString(DateTimeOffset time) {            
            string minute = time.Minute < 10 ? $"0{time.Minute}" : time.Minute.ToString();
            string hour = time.Hour < 10 ? $"0{time.Hour}" : time.Hour.ToString();
            return $"{hour}:{minute}";
        }
    }
}
