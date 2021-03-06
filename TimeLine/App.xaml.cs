﻿using System.Windows;
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
        private TaskbarIcon taskbarIcon;

        public ToastManager ToastManager { get; private set; }

        public static ApplicationSettings ApplicationSettings { get; set; }
        public static SoundPlayer SoundPlayer { get; set; }
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
            SoundPlayer = new SoundPlayer(ApplicationSettings.AppSettings.SoundFilePath, ApplicationSettings.AppSettings.SoundVolume);
            Manager = new Manager(ToastManager);

            taskbarIcon = (TaskbarIcon)FindResource("TrayIcon");

            MainView mainView = new MainView {
                DataContext = new MainViewModel()
            };
        }

        protected override void OnExit(ExitEventArgs e) {
            Logger.Log("Closing Application\n", LogLevel.DEBUG);

            ApplicationSettings.Save();

            taskbarIcon.Dispose(); // Make sure we remove TrayIcon before exiting app.
            base.OnExit(e);
        }

        public static void ExitApplication() {
            Application.Current.Shutdown();
        }
    }
}
