using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using WpfScreenHelper;

namespace TimeLine.Views
{
    /// <summary>
    /// Interaction logic for ToastMainView.xaml
    /// </summary>
    public partial class ToastHolderView : Window
    {
        int width = 500;
        int height = 600;

        public ToastHolderView() {
            InitializeComponent();

            SourceInitialized += (s, e) =>
            {
                var win = new WindowInteropHelper(this);
                win.Owner = GetDesktopWindow();
            };

            this.Width = width;
            this.Height = height;

            this.Left = Screen.PrimaryScreen.Bounds.Right - width;
            this.Top = Screen.PrimaryScreen.Bounds.Bottom - height * 1.05;
        }

        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();
    }
}
