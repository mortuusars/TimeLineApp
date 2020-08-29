using System.Windows;
using System.Windows.Input;
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

            CommandTextBox.PreviewKeyDown += CommandTextBox_PreviewKeyDown;

            //CommandTextBox.Focus();
        }

        private void CommandTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Down) {
                var keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null) {
                    keyboardFocus.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Append suggestion to current TextBox text.
        /// </summary>
        /// <param name="header"></param>
        public void AppendString(string header) {
            var VM = (CommandViewModel)this.DataContext;
            VM.AppendSuggestion(header);
            
            CommandTextBox.CaretIndex = CommandTextBox.Text.Length;
            CommandTextBox.Focus();
        }
    }
}
