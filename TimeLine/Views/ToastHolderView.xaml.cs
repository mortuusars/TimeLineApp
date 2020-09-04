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
    /// Interaction logic for ToastMainView.xaml
    /// </summary>
    public partial class ToastHolderView : Window
    {

        public ToastHolderView() {
            InitializeComponent();

            SourceInitialized += (s, e) =>
            {
                var win = new WindowInteropHelper(this);
                win.Owner = GetDesktopWindow();
            };
        }

        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();
    }
}
