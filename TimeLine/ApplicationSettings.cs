using System;
using System.IO;
using System.Text.Json;

namespace TimeLine
{
    public class ApplicationSettings
    {
        string propertiesFilePath = "settings.json";

        public AppSettings AppSettings;



        public void Load() {
            try {
                string file = File.ReadAllText(propertiesFilePath);
                AppSettings = JsonSerializer.Deserialize<AppSettings>(file);
            }
            catch (System.Exception ex) {
                MortuusLogger.Logger.Log($"Failed to load settings: {ex.Message}.", MortuusLogger.LogLevel.ERROR);
                MortuusLogger.Logger.Log($"Loading default settings", MortuusLogger.LogLevel.ERROR);


                AppSettings = DefaultSettings();
            }
        }



        public void Save() {
            string jsonString = "";

            if (AppSettings != null)
                jsonString = JsonSerializer.Serialize(AppSettings, new JsonSerializerOptions() { WriteIndented = true });
            else {
                jsonString = JsonSerializer.Serialize(DefaultSettings());
                MortuusLogger.Logger.Log($"Settings was null. Saving default settings", MortuusLogger.LogLevel.ERROR);
            }

            try {
                File.WriteAllText(propertiesFilePath, jsonString);
            }
            catch (System.Exception ex) {
                MortuusLogger.Logger.Log($"Failed to save settings: {ex.Message}", MortuusLogger.LogLevel.ERROR);
            }
        }



        private AppSettings DefaultSettings() {
            return new AppSettings() 
            { 
                SoundFilePath = "Resources/Sounds/subtle.wav",
                SoundVolume = 60,

                ToastNotificationDelayInSeconds = 6,

            };
        }
    }
}
