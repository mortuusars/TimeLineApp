using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using MortuusLogger;
using TimeLine.Core;

namespace TimeLine
{
    /// <summary>
    /// Gives ability to save and load information about Application State from file.
    /// </summary>
    public class ApplicationState
    {

        //TODO: Refactor to split reading\writing to separate class. For ability to read\write different things.

        string stateFilePath = "savedstate.json";

        /// <summary>
        /// Returns state data from file, or in case of error - default state.
        /// </summary>
        /// <returns></returns>
        public State ReadState() {            
            State state;
            
            try {
                string file = File.ReadAllText(stateFilePath);
                state = JsonSerializer.Deserialize<State>(file);
            }
            catch (System.Exception ex) {
                Logger.Log($"Failed to restore previous application state: {ex.Message}.", LogLevel.ERROR);
                state = LoadDefaultState();
            }

            return state;
        }

        /// <summary>
        /// Saves current state.
        /// </summary>
        /// <param name="state"></param>
        public void SaveState(State state) {
            SerializeAndWrite(state);
        }




        private State LoadDefaultState() {
            return new State() { TimerRingTime = 0, StopwatchCount = 0, Alarms = null };
        }

        private void SerializeAndWrite(object serializedObject) {
            string jsonString = JsonSerializer.Serialize(serializedObject, new JsonSerializerOptions() { WriteIndented = true });

            try {
                File.WriteAllText(stateFilePath, jsonString);
            }
            catch (System.Exception ex) {
                Logger.Log($"Failed to save application state: {ex.Message}", LogLevel.ERROR);
                MessageBox.Show($"Failed to save application state: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
