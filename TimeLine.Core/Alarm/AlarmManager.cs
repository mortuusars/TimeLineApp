using System;
using System.Collections.Generic;
using MortuusLogger;

namespace TimeLine.Core
{
    public class AlarmManager
    {
        public event EventHandler<string> AlarmRing;

        List<Alarm> Alarms;

        DateTimeOffset CurrentTime;

        public AlarmManager() {
            Alarms = new List<Alarm>();

            CreateCheckTimer();
        }

        public List<Alarm> GetAlarmsList() {
            return Alarms;
        }

        public List<string> GetAlarmsStringList() {
            List<string> alarmsList = new List<string>();

            foreach (var alarm in Alarms) {
                alarmsList.Add(Utilities.TimeToString(alarm.RingTime));
            }

            return alarmsList;
        }

        public void AddAlarm(DateTimeOffset ringTime) {
            Alarms.Add(new Alarm(ringTime));
            Logger.Log($"Alarm added: {Utilities.TimeToString(ringTime)}", LogLevel.INFO);
        }

        public bool ClearAllAlarms() {
            if (Alarms.Count > 0) {
                Alarms = new List<Alarm>();
                Logger.Log("Alarms cleared", LogLevel.INFO);
                return true;
            }
            else {
                Logger.Log("No alarms to clear", LogLevel.INFO);
                return false;
            }

        }

        //TODO: Skip



        private void CreateCheckTimer() {
            System.Timers.Timer checkTimer = new System.Timers.Timer();
            checkTimer.Interval = 10000;
            checkTimer.Elapsed += CheckTimer_Tick;
            checkTimer.Start();
        }

        private void CheckTimer_Tick(object sender, System.Timers.ElapsedEventArgs e) {

            CurrentTime = DateTimeOffset.Now;

            foreach (var alarm in Alarms) {

                if (alarm.HasRangRecently == false) {
                    if (alarm.RingTime.Hour == CurrentTime.Hour && alarm.RingTime.Minute == CurrentTime.Minute) {
                        AlarmRing?.Invoke(this, Utilities.TimeToString(alarm.RingTime));
                        alarm.HasRangRecently = true;
                        Logger.Log($"{Utilities.TimeToString(alarm.RingTime)} Alarm went off", LogLevel.INFO);
                    }
                }

            }
        }
    }
}
