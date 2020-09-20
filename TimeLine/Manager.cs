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

        public RunCommandView RunCommandView { get; private set; }
        public RunCommandViewModel RunCommandViewModel { get; private set; }

        public bool TimerIsRunning { get { return Timer.IsRunning; } }
        public bool StopwatchRunning { get { return StopwatchManager.IsRunning(); } }

        public HistoryViewModel HistoryVM;

        ToastManager ToastManager;

        Timer Timer;
        StopwatchManager StopwatchManager;
        AlarmManager Alarm;



        public Manager(ToastManager toastManager) {
            ToastManager = toastManager;

            InitializeTimer();
            StopwatchManager = new StopwatchManager();
            InitializeAlarm();

            HistoryVM = new HistoryViewModel();

            RestoreApplicationState();
        }



        /// <summary>
        /// Mute or Unmute sound.
        /// </summary>
        private void MuteSound() {
            if (App.SoundPlayer.IsMuted == true) {
                App.SoundPlayer.UnMute();
                ToastManager.ShowToastNotification("TimeLine", "Sound unmuted", Icons.info);
            }
            else {
                App.SoundPlayer.Mute();
                ToastManager.ShowToastNotification("TimeLine", "Sound muted", Icons.info);
            }
        }



        #region State

        private void SaveApplicationState() {

            long timerSavedTime = Timer.RemainingSeconds > 0 ? DateTimeOffset.Now.ToUnixTimeSeconds() + Timer.RemainingSeconds : 0;
            int stopwatchCount = StopwatchManager.GetCount();

            var state = new State() {
                TimerRingTime = timerSavedTime,
                StopwatchCount = stopwatchCount,
                StopwatchRunning = StopwatchManager.IsRunning(),
                StopwatchWindowOpen = StopwatchManager.IsWindowOpen(),
                Alarms = Alarm.GetAlarmsList()
            };

            JsonFileIO json = new JsonFileIO(Properties.StateFilePath);
            try {
                json.Write(state);
            }
            catch (Exception) {
                Logger.Log("Failed to write state to file. Timer, stopwatch and alarms will not be restored on start", LogLevel.ERROR);
            }
        }

        private void RestoreApplicationState() {

            JsonFileIO json = new JsonFileIO(Properties.StateFilePath);

            State state;

            try {
                state = json.Read<State>();
            }
            catch (Exception ex) {
                Logger.Log($"Failed reading state from file: {ex.Message}", LogLevel.ERROR);
                state = new State();
            }

            RestoreTimer(state.TimerRingTime);
            RestoreStopwatch(state.StopwatchCount, state.StopwatchRunning, state.StopwatchWindowOpen);
            RestoreAlarms(state.Alarms);
        }



        // Restore timer if saved TimerTime is not 0, and has not already passed.
        private void RestoreTimer(long _timerRingTime) {

            long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();

            if (_timerRingTime > 0 && _timerRingTime - currentTime > 0) {
                var remaining = _timerRingTime - currentTime;

                try {
                    Timer.Start((int)remaining);
                }
                catch (Exception) {
                    Logger.Log("Incorrect stopwatch starting count", LogLevel.ERROR);
                }

                var remainingString = Utilities.PrettyTime((int)(remaining));
                ToastManager.ShowToastNotification("Timer", $"Restored: {remainingString} remaining", Icons.timer);
            }
        }

        private void RestoreStopwatch(int count, bool isRunning, bool isWindowOpen) {
            if (isWindowOpen) {
                if (count > 0)
                    if (isRunning)
                        StopwatchManager.Start(count);
                    else {
                        StopwatchManager.Start(count);
                        StopwatchManager.Pause();
                    }
            }
        }

        private void RestoreAlarms(List<Alarm> alarms) {

            if (alarms == null)
                return;

            foreach (var alarm in alarms) {
                Alarm.AddAlarm(alarm.RingTime);
            }
        }

        #endregion




        public void AddHistoryItem(HistoryItem historyitem) {
            if (HistoryVM.HistoryList.Count > 10)
                HistoryVM.HistoryList.RemoveAt(0);

            HistoryVM.HistoryList.Add(historyitem);
        }


        #region Command Window

        /// <summary>
        /// Creates and shows RunCommand Window. Closes if window is open.
        /// </summary>
        public void ToggleRunCommandView() {

            if (RunCommandView == null) {
                RunCommandViewModel = new RunCommandViewModel();

                RunCommandView = new RunCommandView() { DataContext = RunCommandViewModel };
                RunCommandView.Show();
                RunCommandView.Activate();
            }
            else {
                RunCommandViewModel.SuggestionsClosing = true;
                RunCommandViewModel.Closing = true;

                RunCommandView?.Close();
                RunCommandView = null;
            }
        }

        #endregion


        #region Parsing Command

        public void ParseInput(string inputText) {
            ParsedCommandData parsedData = new CommandParser().Parse(inputText);
            Logger.Log($"Parsed following data from {inputText} : [{parsedData.MainCommand}], [{parsedData.OperationCommand}], [{parsedData.Hours}], " +
                $"[{parsedData.Minutes}], [{parsedData.Seconds}]", LogLevel.DEBUG);

            RunParsedCommand(parsedData);
            SaveApplicationState();
        }

        private void RunParsedCommand(ParsedCommandData parsedData) {
            if (parsedData.MainCommand == "timer") {
                TimerCommands(parsedData);
            }
            else if (parsedData.MainCommand == "stopwatch") {
                StopwatchCommands(parsedData);

            }
            else if (parsedData.MainCommand == "alarm") {
                AlarmCommands(parsedData);
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
                ToastManager.ShowToastNotification("TimeLine", "Command is not recognized", Icons.error);
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

            //TODO: move to methods
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
                        toastMessage = TimerIsRunning ? "Stopped" : "Timer is not running";
                        icon = TimerIsRunning ? Icons.timer : Icons.error;
                        Timer.Stop();
                        break;
                    }
                case "info": {
                        toastMessage = TimerIsRunning ? $"Remaining { Utilities.PrettyTime(TimerCountdown)}" : "Timer is not running";
                        icon = TimerIsRunning ? Icons.timer : Icons.error;
                        break;
                    }
                // Not recognized command
                default: {
                        toastMessage = "Command is not recognized";
                        icon = Icons.error;
                        break;
                    }
            }

            ToastManager.ShowToastNotification(toastTitle, toastMessage, icon);
        }


        private void Timer_Countdown(object sender, int time) {
            Logger.Log($"Countdown: {time}", LogLevel.DEBUG);
            TimerCountdown = time;
        }

        private void Timer_TimerEnded(object sender, int timeForTimer) {
            Logger.Log($"Timer for {timeForTimer} s. is ended", LogLevel.DEBUG);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ToastManager.ShowToastNotification("Timer", $"{Utilities.PrettyTime(timeForTimer)} has passed.", Icons.timer, IsAlarm: true);
                App.SoundPlayer.Play();
            }));

        }

        #endregion


        #region Stopwatch

        private void StopwatchCommands(ParsedCommandData parsedData) {
            if (parsedData.OperationCommand == "")
                StopwatchManager.ToggleWindow();
            else if (parsedData.OperationCommand == "start")
                StopwatchManager.Start();
            else if (parsedData.OperationCommand == "stop") {
                if (StopwatchManager.Stop() == false)
                    ToastManager.ShowToastNotification("Stopwatch", "Not running", Icons.error);
            }
            else if (parsedData.OperationCommand == "pause")
                StopwatchManager.Pause();
            else if (parsedData.OperationCommand == "close") {
                StopwatchManager.Close();
            }
            else if (parsedData.OperationCommand == "reset") {
                StopwatchManager.Reset();
            }
        }

        #endregion


        #region Alarm

        private void InitializeAlarm() {
            Alarm = new AlarmManager();
            Alarm.AlarmRing += Alarm_AlarmRing;
        }

        private void AlarmCommands(ParsedCommandData parsedData) {
            if (parsedData.OperationCommand == "") {
                try {
                    var ringTime = DateTimeOffset.Parse($"{parsedData.Hours}:{parsedData.Minutes}");
                    Alarm.AddAlarm(ringTime);
                    ToastManager.ShowToastNotification("Alarm", $"Set to {Utilities.TimeToString(ringTime)}", Icons.alarm);
                }
                catch (Exception) {
                    ToastManager.ShowToastNotification("Alarm", "Enterd time is incorrect", Icons.error);
                    Logger.Log("Time for alarm is incorrect", LogLevel.WARN);
                }
            }
            else if (parsedData.OperationCommand == "in") {
                var time = DateTimeOffset.Now.AddHours(parsedData.Hours).AddMinutes(parsedData.Minutes);

                Alarm.AddAlarm(time);
                ToastManager.ShowToastNotification("Alarm", $"Set to {Utilities.TimeToString(time)}", Icons.alarm);
            }
            else if (parsedData.OperationCommand == "clear") {
                string message = Alarm.ClearAllAlarms() == true ? "All alarms was cleared" : "Nothing to clear";
                ToastManager.ShowToastNotification("Alarm", message, Icons.alarm);
            }
            else if (parsedData.OperationCommand == "list") {
                var list = Alarm.GetAlarmsList();

                foreach (var alarm in list) {                    
                    string isDisabled = alarm.Disabled ? "(disabled)" : "";
                    ToastManager.ShowToastNotification("Alarm", $"{Utilities.TimeToString(alarm.RingTime)}{isDisabled}", Icons.alarm);
                }
            }
        }

        private void Alarm_AlarmRing(object sender, string e) {
            Application.Current.Dispatcher.Invoke(new Action(() => { ToastManager.ShowToastNotification("Alarm", e, Icons.alarm, IsAlarm: true); }));
            App.SoundPlayer.Play();
        }

        #endregion
    }
}
