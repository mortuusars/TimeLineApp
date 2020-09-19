using System.Windows;
using System.Windows.Input;

namespace TimeLine.Models
{
    public class Suggestion
    {
        public string Header { get; set; }

        public int SuggestionButtonHeight { get { return RunCommandViewModel.suggestionButtonHeight; } }

        public CornerRadius CornerRadius { get; set; }

        public ICommand AddSuggestionCommand { get; set; }

        public Suggestion(string header) {
            Header = header;

            AddSuggestionCommand = new RelayCommand(act => { App.Manager.RunCommandView.AppendString(Header); });
        }
    }
}
