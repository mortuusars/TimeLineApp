using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
        public RunCommandView() {
            InitializeComponent();

            CommandTextBox.PreviewKeyDown += CommandTextBox_PreviewKeyDown;

        }


        public void AppendString(string header) {
            throw new NotImplementedException();
        }


        private void CommandTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == Key.Down) {
                var keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null) {
                    keyboardFocus.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
                e.Handled = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape)
                GetService.Manager.ShowOrCloseCommandView();
        }
    }
}
