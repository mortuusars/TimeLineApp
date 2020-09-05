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
    }
}
