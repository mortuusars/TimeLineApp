using System.Windows;
using TimeLine.ViewModels;

namespace TimeLine.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CommandView : Window
    {
        public CommandView() {
            InitializeComponent();

            CommandTextBox.Focus();
        }
    }
}
