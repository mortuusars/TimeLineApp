using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLine
{
    public static class IconHelper
    {
        public static string GetIconPath(Icons icon) {
            if (icon == Icons.alarm)
                return "pack://application:,,,/Resources/Icons/alarm_64.png";
            else if (icon == Icons.clock)
                return "pack://application:,,,/Resources/Icons/clock_64.png";            
            else if (icon == Icons.stopwatch)
                return "pack://application:,,,/Resources/Icons/stopwatch_64.png";
            else if (icon == Icons.timer)
                return "pack://application:,,,/Resources/Icons/timer_64.png";
            else if (icon == Icons.error)
                return "pack://application:,,,/Resources/Icons/error_64.png";
            else
                return "pack://application:,,,/Resources/Icons/info_64.png";
        }
    }
}
