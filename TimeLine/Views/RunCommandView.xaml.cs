using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

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

        bool IsClosing;

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
                App.Manager.ToggleRunCommandView();
        }


        private void GhostTextStartAnimation_Completed(object sender, EventArgs e) {
            GhostTextBlock.Opacity = 1;
        }

        private void GhostTextEndAnimation_Completed(object sender, EventArgs e) {
            GhostTextBlock.Opacity = 0;
        }

        protected override void OnClosing(CancelEventArgs e) {

            if (IsClosing) {
                e.Cancel = false;
                return;
            }
            else
                e.Cancel = true;

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = ((Duration)App.Current.FindResource("WindowFadeDuration")).TimeSpan;
            dispatcherTimer.Tick += (s, e) => { IsClosing = true; this.Close(); dispatcherTimer.Stop(); };

            dispatcherTimer.Start();

            base.OnClosing(e);
        }
    }
}
