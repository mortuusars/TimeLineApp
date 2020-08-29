using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using MortuusLogger;
using TimeLine.Core;
using TimeLine.Models;
using TimeLine.ViewModels;
using TimeLine.Views;

namespace TimeLine
{
    public class Manager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public CommandView CommandView { get; private set; }        
        public CommandViewModel CurrentCommandVM { get; private set; }

        ToastMainView toastMain;

        public ObservableCollection<ToastControlViewModel> ToastList { get; set; }


        // Services
        Timer Timer;
        // Stopwatch
        // Alarm

        NetCoreAudio.Player soundPlayer = new NetCoreAudio.Player();

        public Manager() {
            ToastList = new ObservableCollection<ToastControlViewModel>();

            toastMain = new ToastMainView();
            toastMain.Left = WpfScreenHelper.Screen.PrimaryScreen.Bounds.Right - toastMain.Width;
            toastMain.Top = WpfScreenHelper.Screen.PrimaryScreen.Bounds.Bottom - toastMain.Height * 1.05;
            //TODO: Proper ViewModel for ToastMainWindow
            toastMain.DataContext = this;
            toastMain.Show();

            CreateTimer();
        }

        private void CreateTimer() {
            Timer = new Timer();
            Timer.TimerEnded += Timer_TimerEnded;
            Timer.Countdown += Timer_Countdown;
        }


        public void PlaySound() {
            if (soundPlayer.Playing) {
                soundPlayer.Stop();
                PlaySound();
            }
            else {
                soundPlayer.Play("Resources/Sounds/subtle.wav");
            }
        }

        public void StopSound() {
            soundPlayer.Stop();
        }





        #region Toast

        //TODO: Closing Animation?
        public void ShowToastNotification(string title, string message, Icons icon, bool IsAlarm = false) {

            ToastControlViewModel newToast = new ToastControlViewModel(title, message, icon);

            ToastList.Add(newToast);

            if (IsAlarm == false) {
                // Closing Timer;
                System.Timers.Timer toastTimer = new System.Timers.Timer();
                toastTimer.Interval = 4000;
                toastTimer.Elapsed += (s, e) => { Application.Current.Dispatcher.Invoke(new Action(() => { CloseToast(newToast); })); toastTimer.Stop(); toastTimer.Dispose(); };
                toastTimer.Start();
            }
            else {
                PlaySound();
            }
        }

        public void CloseToast(ToastControlViewModel toast, bool fromCommand = false) {
            toast.Close();
            if (fromCommand)
                StopSound();
        }

        public void RemoveToastFromList(ToastControlViewModel toast) {
            ToastList.Remove(toast);
        }

        #endregion




        public void ShowOrCloseCommandView() {
            if (CommandView == null) {
                CurrentCommandVM = new CommandViewModel();
                
                CommandView = new CommandView() { DataContext = CurrentCommandVM };                
                CommandView.Show();
            }
            else {
                CommandView.Close();
                CommandView = null;
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
                    "Started for 3 minutesStarted for 3 minutesStarted for 3 minutesStarted for 3 minutes", Icons.timer);
            }
            else if (parsedData.MainCommand == "exit") {                
                App.ExitApplication();
            }
        }







        #region Timer

        //TODO: Better check for null and refactor showing toasts

        public int TimerCountdown { get; private set; }

        private void TimerCommands(ParsedCommandData parsed) {

            string toastTitle = "Timer";
            string toastMessage;
            Icons icon = Icons.timer;

            int overallSeconds = parsed.OverallSeconds();

            switch (parsed.OperationCommand) {
                case "": {
                        if (overallSeconds <= 0)
                            toastMessage = "Cannot set timer to negative time";
                        else {
                            Timer.Start(overallSeconds);
                            toastMessage = $"Started for { Utilities.PrettyTime(overallSeconds)}";
                        }
                        break;
                    }
                case "add": {
                        if (TimerCountdown <= 0) {
                            toastMessage = "Timer is not running";
                        }
                        else {
                            var timeToAdd = parsed.OverallSeconds();
                            Timer.Add(timeToAdd);
                            toastMessage = timeToAdd > 0 ? $"Added {Utilities.PrettyTime(timeToAdd)} to timer" : $"Subtracted {Utilities.PrettyTime(timeToAdd, removeMinusSign: true)} from timer";
                        }
                        break;
                    }
                case "stop": {
                        if (TimerCountdown <= 0) {
                            toastMessage = "Timer is not running";
                        }
                        else {
                            Timer.Stop();
                            toastMessage = $"Stopped";
                        }
                        break;
                    }
                case "info": {
                        if (TimerCountdown <= 0) {
                            toastMessage = "Timer is not running";
                        }
                        else {
                            toastMessage = $"Remaining { Utilities.PrettyTime(TimerCountdown)}";
                        }
                        break;
                    }
                // Not recognized command
                default: {
                        toastMessage = "Command is not recognized";
                        break;
                    }
            }

            ShowToastNotification(toastTitle, toastMessage, icon);
        }


        private void Timer_Countdown(object sender, int time) {
            Logger.Log($"Countdown: {time}", LogLevel.DEBUG);
            TimerCountdown = time;
        }

        private void Timer_TimerEnded(object sender, int timeForTimer) {
            Logger.Log($"Timer for {timeForTimer} s. is ended", LogLevel.DEBUG);

            Application.Current.Dispatcher.Invoke(new Action(() => { ShowToastNotification("Timer", $"{Utilities.PrettyTime(timeForTimer)} has passed.", Icons.timer, IsAlarm: true); }));

        }

        #endregion
    }
}
