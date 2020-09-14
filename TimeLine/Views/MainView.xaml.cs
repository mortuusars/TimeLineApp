using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Hardcodet.Wpf.TaskbarNotification;

namespace TimeLine.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public static Window Main;

        public MainView() {            
            InitializeComponent();
            Main = this;

            this.Show();
            this.Hide();

            //var temp = App.Manager.GetHashCode();
        }

        private void GlobalHotkey_Handle() {
            App.Manager.ShowOrCloseCommandView();
        }

        #region Global Hotkey


        #region Key Defining

        //Modifiers:

        //private const uint MOD_NONE = 0x0000; //[NONE]
        private const uint MOD_ALT = 0x0001; //ALT
        //private const uint MOD_CONTROL = 0x0002; //CTRL
        //private const uint MOD_SHIFT = 0x0004; //SHIFT

        //private const uint MOD_WIN = 0x0008; //WINDOWS doesn't work for me. Propably because microsoft reserve win-hotkeys for themselves.
        // There should be a way to make this work.

        // Keys
        private const uint VK_T = 0x54; // T

        #endregion


        #region Register Global Hotkey 

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;



        private HwndSource source;

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);

            IntPtr handle = new WindowInteropHelper(this).Handle;
            source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);

            //RegisterHotKey(handle, HOTKEY_ID, MOD_ALT + MOD_CONTROL + MOD_SHIFT, VK_T); // ALT + CONTROL + SHIFT + T

            // Register ALT + T
            RegisterHotKey(handle, HOTKEY_ID, MOD_ALT, VK_T);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            const int WM_HOTKEY = 0x0312;
            switch (msg) {
                case WM_HOTKEY:
                    switch (wParam.ToInt32()) {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_T) {
                                MortuusLogger.Logger.Log("Global Hotkey Pressed", MortuusLogger.LogLevel.DEBUG);
                                GlobalHotkey_Handle();
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        #endregion

        #endregion
    }
}
