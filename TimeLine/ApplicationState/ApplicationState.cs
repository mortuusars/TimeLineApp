using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using MortuusLogger;
using TimeLine.Core;

namespace TimeLine
{
    public class ApplicationState
    {
        string stateFilePath = "savedstate.json";

        public State State { get; set; }

        //TODO: Load
        public void ReadState() {            
            try {
                string file = File.ReadAllText(stateFilePath);
                State = JsonSerializer.Deserialize<State>(file);
            }
            catch (System.Exception ex) {
                Logger.Log($"Failed to restore previous application state: {ex.Message}.", LogLevel.ERROR);
            }

        }

        public void SaveCurrentState(int timerRingTime, int stopwatchCount, List<Alarm> alarms) {            
            State = new State() { TimerRingTime = timerRingTime, StopwatchCount = stopwatchCount, Alarms = alarms };   
            SerializeAndWrite();
        }




        private void SerializeAndWrite() {
            string jsonString = JsonSerializer.Serialize(State, new JsonSerializerOptions() { WriteIndented = true });

            try {
                File.WriteAllText(stateFilePath, jsonString);
            }
            catch (System.Exception ex) {
                MessageBox.Show($"Failed to save application state: {ex.Message}");
                Logger.Log($"Failed to save application state: {ex.Message}", LogLevel.ERROR);
            }
        }
    }
}
