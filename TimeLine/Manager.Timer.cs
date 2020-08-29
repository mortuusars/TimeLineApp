using System;
using System.Windows;
using MortuusLogger;
using TimeLine.Core;

namespace TimeLine
{
    public partial class Manager
    {
        //TODO: Better check for null and refactor showing toasts

        Timer Timer;

        public int TimerCountdown { get; private set; }

        private void TimerCommands(ParsedCommandData parsedData) {
            if (parsedData.OperationCommand == "" && parsedData.GetOverallSeconds() != 0) {
                if (Timer != null) {
                    DeleteTimer();
                }
                else {
                    CreateTimer();
                    Timer.Start(parsedData.GetOverallSeconds());
                    ShowToastNotification("Timer", $"Started for { Utilities.IntToPrettyTime(parsedData.GetOverallSeconds())}", IconType.timer);
                }                
            }
            else if (parsedData.OperationCommand == "add") {
                AddToTimer(parsedData);
            }
            else if (parsedData.OperationCommand == "stop") {
                if (Timer != null) {
                    DeleteTimer();
                    ShowToastNotification("Timer", $"Stopped. Remaining { Utilities.IntToPrettyTime(TimerCountdown)}", IconType.timer);
                }
                else
                    ShowToastNotification("Timer", $"Timer is not running", IconType.timer);

            }
            else if (parsedData.OperationCommand == "info") {
                if (Timer != null)
                    ShowToastNotification("Timer", $"Remaining { Utilities.IntToPrettyTime(TimerCountdown)}", IconType.timer);
                else
                    ShowToastNotification("Timer", $"Timer is not running", IconType.timer);
            }
        }
        
        private void CreateTimer() {
            Timer = new Timer();
            Timer.TimerEnded += Timer_TimerEnded;
            Timer.Countdown += Timer_Countdown;
        }

        private void AddToTimer(ParsedCommandData parsedData) {
            if (Timer != null) {
                var timeToAdd = parsedData.GetOverallSeconds();

                Timer.Add(timeToAdd);

                if (timeToAdd < 0)
                    ShowToastNotification("Timer", $"Subtracted { Utilities.IntToPrettyTime(parsedData.GetOverallSeconds())} from timer", IconType.timer);
                else
                    ShowToastNotification("Timer", $"Added { Utilities.IntToPrettyTime(parsedData.GetOverallSeconds())} to timer", IconType.timer);
            }
            else {
                ShowToastNotification("Timer", $"Timer is not running", IconType.timer);
                return;
            }
        }

        private void DeleteTimer() {
            Timer.Stop();
            Timer = null;
        }


        private void Timer_Countdown(object sender, int time) {
            Logger.Log($"Countdown: {time}", LogLevel.DEBUG);
            TimerCountdown = time;
        }

        private void Timer_TimerEnded(object sender, int timeForTimer) {
            Logger.Log($"Timer for {timeForTimer} s. is ended", LogLevel.DEBUG);

            Application.Current.Dispatcher.Invoke(new Action(() => { ShowToastNotification("Timer", $"{Utilities.IntToPrettyTime(timeForTimer)} has passed.", IconType.timer); }));
            DeleteTimer();
        }
    }
}
