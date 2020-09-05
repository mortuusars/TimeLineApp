using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLine.Models
{
    public class HistoryItem
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Icon { get; set; }
        public string Time { get; set; }

        public HistoryItem(string title, string message, Icons icon) {
            Title = title;
            Message = message;
            Icon = IconHelper.GetIconPath(icon);
            Time = CurrentTimeString();
        }

        private string CurrentTimeString() {
            var time = DateTimeOffset.Now;
            return $"{time.Hour}:{time.Minute}";
        }
    }
}
