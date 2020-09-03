﻿using System;
using System.Collections.ObjectModel;
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


        // Services
        Timer Timer;
        // Stopwatch
        // Alarm


        public Manager() { 

            InitializeTimer();

        }

        


        #region Sound

        public void PlaySound() {
            SoundPlayer.Stop();
            SoundPlayer.Play();
        }

        public void StopSound() {
            SoundPlayer.Stop();
        }

        #endregion


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
                GetService.ToastManager.ShowToastNotification("Stopwatch", "test", Icons.stopwatch);
            }
            else if (parsedData.MainCommand == "alarm") {
                //TODO: alarm
                GetService.ToastManager.ShowToastNotification("Alarm", "test", Icons.alarm);
            }
            else if (parsedData.MainCommand == "mute") {
                // Mute
                GetService.ToastManager.ShowToastNotification("Timer", "Started for 3 minutes" +
                    "Started for 3 minutesStarted for 3 minutesStarted for 3 minutesStarted for 3 minutesStarted for 3 minutes" +
                    "Started for 3 minutesStarted for 3 minutesStarted for 3 minutesStarted for 3 minutes", Icons.timer);
            }
            else if (parsedData.MainCommand == "exit") {
                App.ExitApplication();
            }
            else {
                GetService.ToastManager.ShowToastNotification("TimeLine", "Command is not recognized", Icons.info);
            }
        }







        #region Timer

        //TODO: Better check for null and refactor showing toasts

        public int TimerCountdown { get; private set; }

        /// <summary>
        /// Invoked from constructor.
        /// </summary>
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
                        if (overallSeconds <= 0)
                            toastMessage = "Cannot set timer to that time";
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

            GetService.ToastManager.ShowToastNotification(toastTitle, toastMessage, icon);
        }


        private void Timer_Countdown(object sender, int time) {
            Logger.Log($"Countdown: {time}", LogLevel.DEBUG);
            TimerCountdown = time;
        }

        private void Timer_TimerEnded(object sender, int timeForTimer) {
            Logger.Log($"Timer for {timeForTimer} s. is ended", LogLevel.DEBUG);

            Application.Current.Dispatcher.Invoke(new Action(() => { 
                GetService.ToastManager.ShowToastNotification("Timer", $"{Utilities.PrettyTime(timeForTimer)} has passed.", Icons.timer, IsAlarm: true); }));

        }

        #endregion


        #region Stopwatch

        #endregion


        #region Alarm

        #endregion
    }
}
