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

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            // Log setup
            Logger.LoggerInstance = new FileLogger("log.txt");
            Logger.LoggerInstance.GotException += (s, e) => { MessageBox.Show($"Error writing to log file: {e.Message}"); };
            Logger.LoggingLevel = LogLevel.DEBUG;


            // Dependency injection


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
