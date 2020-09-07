using System;
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
        AlarmManager Alarm;


        public Manager() {
            InitializeTimer();
            InitializeAlarm();
           
            HistoryVM = new HistoryViewModel();
        }








        private void InitializeAlarm() {
            Alarm = new AlarmManager();
            Alarm.AlarmRing += Alarm_AlarmRing;
        }



        private void AlarmActions(ParsedCommandData parsedData) {
            
            if (parsedData.OperationCommand == "") {
                try {
                    var ringTime = DateTimeOffset.Parse($"{parsedData.Hours}:{parsedData.Minutes}");
                    Alarm.AddAlarm(ringTime);
                    GetService.ToastManager.ShowToastNotification("Alarm", $"Set to {Utilities.TimeToString(ringTime)}", Icons.alarm);
                }
                catch (Exception) {
                    GetService.ToastManager.ShowToastNotification("Alarm", "Enterd time is incorrect", Icons.error);
                    Logger.Log("Time for alarm is incorrect", LogLevel.WARN);
                }
            }
            else if (parsedData.OperationCommand == "clear") {
                string message = Alarm.ClearAllAlarms() == true ? "All alarms was cleared" : "Nothing to clear";
                GetService.ToastManager.ShowToastNotification("Alarm", message, Icons.alarm);
            }
            else if (parsedData.OperationCommand == "list") {
                var list = Alarm.GetAlarmsList();

                foreach (var item in list) {
                    Logger.Log(item, LogLevel.INFO);
                }
            }
        }






        private void Alarm_AlarmRing(object sender, string e) {
            Application.Current.Dispatcher.Invoke(new Action(() => { GetService.ToastManager.ShowToastNotification("Alarm", e, Icons.alarm, IsAlarm: true); }));
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
                AlarmActions(parsedData);
            }
            else if (parsedData.MainCommand == "mute") {
                MuteSound();
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


        /// <summary>
        /// Mute or Unmute sound.
        /// </summary>
        private static void MuteSound() {
            if (GetService.SoundPlayer.IsMuted == true) {
                GetService.SoundPlayer.UnMute();
                GetService.ToastManager.ShowToastNotification("TimeLine", "Sound unmuted", Icons.info);
            }
            else {
                GetService.SoundPlayer.Mute();
                GetService.ToastManager.ShowToastNotification("TimeLine", "Sound muted", Icons.info);
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
