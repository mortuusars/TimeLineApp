using System.Windows;
using System.Windows.Input;

namespace TimeLine.Models
{
    public class Suggestion
    {
        public string Header { get; set; }

        public CornerRadius CornerRadius { get; set; }

        public ICommand AddSuggestionCommand { get; set; }

        public Suggestion(string header) {
            Header = header;

            AddSuggestionCommand = new RelayCommand(act => { GetService.Manager.CommandView.AppendString(Header); });
        }
    }
}
