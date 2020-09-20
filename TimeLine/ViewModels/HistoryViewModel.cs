using System.Collections.ObjectModel;
using System.ComponentModel;
using TimeLine.Models;

namespace TimeLine.ViewModels
{
    public class HistoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<HistoryItem> HistoryList { get; set; }

        public HistoryViewModel() {
            HistoryList = new ObservableCollection<HistoryItem>();
        }

        public void AddItem(string title, string message, Icons icon) {
            if (HistoryList.Count > 20)
                HistoryList.RemoveAt(0);

            var historyItem = new HistoryItem(title, message, icon);

            HistoryList.Add(historyItem);
        }
    }
}
