using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TimeLine.Views
{
    /// <summary>
    /// Interaction logic for RunCommandView.xaml
    /// </summary>
    public partial class RunCommandView : Window
    {
        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();

        RunCommandViewModel ViewModel;

        public RunCommandView() {
            InitializeComponent();

            // Remove window from Alt+Tab
            SourceInitialized += (s, e) =>
            {
                var win = new WindowInteropHelper(this);
                win.Owner = GetDesktopWindow();
            };

            CommandTextBox.PreviewKeyDown += CommandTextBox_PreviewKeyDown;
        }





        /// <summary>
        /// Append suggestion to current TextBox text.
        /// </summary>
        /// <param name="header"></param>
        public void AppendString(string header) {
            ViewModel = (RunCommandViewModel)this.DataContext;
            ViewModel.AppendSuggestion(header);

            CommandTextBox.CaretIndex = CommandTextBox.Text.Length;
            CommandTextBox.Focus();
        }


        private void CommandTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == Key.Down) {
                var keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null) {
                    keyboardFocus.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Up) {
                var keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null) {
                    keyboardFocus.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                }
                e.Handled = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape)
                GetService.Manager.ShowOrCloseCommandView();
        }

        private void FoldingAnimationCompleted(object sender, EventArgs e) {
            var binding = new Binding("SuggestionsHeight");
            BindingOperations.SetBinding(SuggestionsBorder, Border.HeightProperty, binding);
            SuggestionsBorder.Opacity = 1;
        }
    }
}
