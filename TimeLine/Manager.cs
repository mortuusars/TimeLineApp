using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using MortuusLogger;
using TimeLine.Core;
using TimeLine.Models;
using TimeLine.ViewModels;
using TimeLine.Views;

namespace TimeLine
{
    public partial class Manager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        CommandView commandView;
        ToastMainView toastMain;

        public ObservableCollection<ToastContent> ToastList { get; set; }

        public Manager() {
            ToastList = new ObservableCollection<ToastContent>();

            toastMain = new ToastMainView();
            toastMain.Left = WpfScreenHelper.Screen.PrimaryScreen.Bounds.Right - toastMain.Width;
            toastMain.Top = WpfScreenHelper.Screen.PrimaryScreen.Bounds.Bottom - toastMain.Height * 1.05;
            //TODO: Proper ViewModel for ToastMainWindow
            toastMain.DataContext = this;
            toastMain.Show();
        }



        #region Toast

        //TODO: Closing Animation?
        public void ShowToastNotification(string title, string message, IconType icon) {

            ToastContent newToast = new ToastContent(title, message, icon);

            ToastList.Add(newToast);
            
            // Closing Timer;
            System.Timers.Timer toastTimer = new System.Timers.Timer();
            toastTimer.Interval = 4000;
            toastTimer.Elapsed += (s, e) => { Application.Current.Dispatcher.Invoke(new Action(() => { CloseToast(newToast); })); toastTimer.Stop(); toastTimer.Dispose(); };
            toastTimer.Start();
        }

        public void CloseToast(ToastContent toast) {
            ToastList.Remove(toast);
        }

        #endregion




        public void ShowOrCloseCommandView() {
            if (commandView == null) {
                Logger.Log("Showing CommandWindow:", LogLevel.DEBUG);
                commandView = new CommandView {
                    DataContext = new CommandViewModel()
                };
                commandView.Show();
            }
            else {
                Logger.Log("Closing CommandWindow..", LogLevel.DEBUG);
                commandView.Close();
                commandView = null;
            }
        }

        public void ParseInput(string inputText) {
            ParsedCommandData parsedData = new CommandParser().Parse(inputText);
            Logger.Log($"Parsed following data from {inputText} : [{parsedData.MainCommand}], [{parsedData.OperationCommand}], [{parsedData.Hours}], " +
                $"[{parsedData.Minutes}], [{parsedData.Seconds}]", LogLevel.DEBUG);

            RunParsedCommand(parsedData);
        }

        private void RunParsedCommand(ParsedCommandData parsedData) {
            if (parsedData.MainCommand == "timer") {
                TimerCommands(parsedData);
            }
            else if (parsedData.MainCommand == "stopwatch") {
                //TODO: Stopwatch
            }
            else if (parsedData.MainCommand == "alarm") {
                //TODO: alarm
            }
            else if (parsedData.MainCommand == "mute") {
                // Mute
                ShowToastNotification("Timer", "Started for 3 minutes" +
                    "Started for 3 minutesStarted for 3 minutesStarted for 3 minutesStarted for 3 minutesStarted for 3 minutes" +
                    "Started for 3 minutesStarted for 3 minutesStarted for 3 minutesStarted for 3 minutes", IconType.timer);
            }
            else if (parsedData.MainCommand == "exit") {
                Logger.Log("Exiting Application", LogLevel.DEBUG);
                App.ExitApplication();
            }
        }
    }
}
