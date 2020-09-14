using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Hardcodet.Wpf.TaskbarNotification.Interop;
using MortuusLogger;
using TimeLine.ViewModels;
using TimeLine.Views;

namespace TimeLine
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainView mainView;
        private TaskbarIcon taskbarIcon;

        public static ApplicationSettings ApplicationSettings { get; set; }
        public static SoundPlayer SoundPlayer { get; set; }
        public static ToastManager ToastManager { get; set; }
        public static Manager Manager { get; set; }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            // Log setup
            Logger.LoggerInstance = new FileLogger("log.txt");
            Logger.LoggerInstance.GotException += (s, e) => { MessageBox.Show($"Error writing to log file: {e.Message}"); };
            Logger.LoggingLevel = LogLevel.INFO;

            
            ApplicationSettings = new ApplicationSettings();
            ApplicationSettings.Load();

            ToastManager = new ToastManager();
            SoundPlayer = new SoundPlayer();
            Manager = new Manager();

            taskbarIcon = (TaskbarIcon)FindResource("TrayIcon");

            mainView = new MainView {
                DataContext = new MainViewModel()
            };
        }

        protected override void OnExit(ExitEventArgs e) {
            Logger.Log("Closing Application\n", LogLevel.DEBUG);

            taskbarIcon.Dispose(); // Make sure we remove TrayIcon before exiting app.
            base.OnExit(e);
        }

        public static void ExitApplication() {
            Application.Current.Shutdown();
        }
    }
}
