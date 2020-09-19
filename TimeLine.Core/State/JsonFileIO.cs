using System;
using System.IO;
using System.Text.Json;

namespace TimeLine.Core
{
    public class JsonFileIO
    {
        string _filePath;

        public JsonFileIO(string filePath) {
            _filePath = filePath;
        }

        /// <summary>
        /// Read file and deserialize info to specified object.
        /// </summary>
        /// <exception cref="Exception">Thrown when reading file failed.</exception>
        public dynamic Read<T>() {
            try {
                string file = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<T>(file);
            }
            catch (Exception) {
                throw;
            }
        }

        /// <summary>
        /// Serialize and write object to file.
        /// </summary>
        /// <param name="objectToWrite">Object that will be serialized and written to file.</param>
        /// /// <exception cref="Exception">Thrown when reading file failed.</exception>
        public void Write(object objectToWrite) {
            string serializedObject = JsonSerializer.Serialize(objectToWrite, new JsonSerializerOptions() { WriteIndented = true });

            try {
                var directoryPath = Path.GetDirectoryName(_filePath);
                Directory.CreateDirectory(directoryPath);
                
                File.WriteAllText(_filePath, serializedObject);
            }
            catch (Exception) {
                throw;
            }
        }
    }
}
