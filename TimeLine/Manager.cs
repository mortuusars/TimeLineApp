using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
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

        public RunCommandView CommandView { get; private set; }
        public RunCommandViewModel CurrentCommandVM { get; private set; }

        public bool TimerIsRunning { get { return Timer.IsRunning; } }


        public HistoryViewModel HistoryVM;

        Timer Timer;
        StopwatchView StopwatchView;
        // Alarm


        public Manager() {
            InitializeTimer();

            HistoryVM = new HistoryViewModel();
        }

        public void AddHistoryItem(HistoryItem historyitem) {
            if (HistoryVM.HistoryList.Count > 10)
                HistoryVM.HistoryList.RemoveAt(0);

            HistoryVM.HistoryList.Add(historyitem);
        }


        #region Command Window

        /// <summary>
        /// Creates and shows Command Window. Closes if window is open. Closing begins animation.
        /// </summary>
        public void ShowOrCloseCommandView() {

            if (CommandView == null) {
                CurrentCommandVM = new RunCommandViewModel();

                CommandView = new RunCommandView() { DataContext = CurrentCommandVM };
                CommandView.Show();
                CommandView.Activate();
            }
            else {
                //TODO: closing animation
                CurrentCommandVM.Closing = true;

                DispatcherTimer dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = ((Duration)App.Current.FindResource("WindowFadeDuration")).TimeSpan;
                dispatcherTimer.Tick += (s, e) => { CloseCommandWindow(); dispatcherTimer.Stop(); };

                dispatcherTimer.Start();
            }
        }

        private void CloseCommandWindow() {
            if (CommandView != null) {
                CommandView.Close();
                CommandView = null;
            }
        }

        #endregion


        #region Parsing Command

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
                
                if (StopwatchView == null) {
                    StopwatchView = new StopwatchView();
                    StopwatchView.DataContext = new StopwatchViewModel();

                    StopwatchView.Show();
                }
                else 
                    StopwatchView = null;

            }
            else if (parsedData.MainCommand == "alarm") {
                //TODO: alarm
                GetService.ToastManager.ShowToastNotification("Alarm", "test", Icons.alarm);
            }
            else if (parsedData.MainCommand == "mute") {

                if (GetService.SoundPlayer.IsMuted == true) {
                    GetService.SoundPlayer.UnMute();
                    GetService.ToastManager.ShowToastNotification("TimeLine", "sound unmuted", Icons.info);
                }
                else {
                    GetService.SoundPlayer.Mute();
                    GetService.ToastManager.ShowToastNotification("TimeLine", "sound muted", Icons.info);
                }
            }
            else if (parsedData.MainCommand == "history") {
                HistoryView history = new HistoryView() { DataContext = HistoryVM };
                history.Show();
            }
            else if (parsedData.MainCommand == "exit") {
                App.ExitApplication();
            }
            else {
                GetService.ToastManager.ShowToastNotification("TimeLine", "Command is not recognized", Icons.error);
            }
        }

        #endregion


        #region Timer

        //TODO: Better check for null and refactor showing toasts

        public int TimerCountdown { get; private set; }

        
        private void InitializeTimer() {
            Timer = new Timer();
            Timer.TimerEnded += Timer_TimerEnded;
            Timer.Countdown += Timer_Countdown;
        }

        private void TimerCommands(ParsedCommandData parsed) {

            string toastTitle = "Timer";
            string toastMessage;
            Icons icon = Icons.timer;

            int overallSeconds = parsed.OverallSeconds();

            switch (parsed.OperationCommand) {
                case "": {
                        if (overallSeconds <= 0) {
                            toastMessage = "Cannot set timer to that time";
                            icon = Icons.error;
                        }
                        else {
                            if (Timer.IsRunning)
                                Timer.Stop();
                            Timer.Start(overallSeconds);
                            toastMessage = $"Started for { Utilities.PrettyTime(overallSeconds)}";
                        }
                        break;
                    }
                case "add": {
                        if (TimerCountdown <= 0) {
                            toastMessage = "Timer is not running";
                            icon = Icons.error;
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
                            icon = Icons.error;
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
                            icon = Icons.error;
                        }
                        else {
                            toastMessage = $"Remaining { Utilities.PrettyTime(TimerCountdown)}";
                        }
                        break;
                    }
                // Not recognized command
                default: {
                        toastMessage = "Command is not recognized";
                        icon = Icons.error;
                        break;
                    }
            }

            GetService.ToastManager.ShowToastNotification(toastTitle, toastMessage, icon);
        }


        private void Timer_Countdown(object sender, int time) {
            Logger.Log($"Countdown: {time}", LogLevel.DEBUG);
            TimerCountdown = time;
        }

        private void Timer_TimerEnded(object sender, int timeForTimer) {
            Logger.Log($"Timer for {timeForTimer} s. is ended", LogLevel.DEBUG);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                GetService.ToastManager.ShowToastNotification("Timer", $"{Utilities.PrettyTime(timeForTimer)} has passed.", Icons.timer, IsAlarm: true);
            }));

        }

        #endregion


        #region Stopwatch

        #endregion


        #region Alarm

        #endregion
    }
}
