using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using TaskTrackerCLI.Models;

namespace TaskTrackerCLI.Services
{
    /// <summary>
    /// Handles reading and writing task data to a JSON file.
    /// Acts as a simple persistence layer (file-based storage).
    /// </summary>
    public class FileService
    {
        /// <summary>
        /// Path to the JSON file where tasks are stored.
        /// Uses application base directory so file works regardless of execution location.
        /// </summary>
        private readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "tasks.json");

        /// <summary>
        /// Reads tasks from the JSON file.
        /// If file does not exist, it creates an empty JSON array.
        /// Handles invalid or empty JSON safely.
        /// </summary>
        public List<TaskItem> ReadTasks()
        {
            try
            {
                // Ensure file exists; if not, initialize with empty array
                if (!File.Exists(FilePath))
                {
                    File.WriteAllText(FilePath, "[]");
                }

                var json = File.ReadAllText(FilePath);

                // If file is empty, return empty list instead of crashing
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<TaskItem>();
                }

                // Deserialize JSON into list of tasks
                return JsonSerializer.Deserialize<List<TaskItem>>(json)
                       ?? new List<TaskItem>();
            }
            catch
            {
                // If JSON is corrupted, reset file to prevent app crash
                File.WriteAllText(FilePath, "[]");
                return new List<TaskItem>();
            }
        }

        /// <summary>
        /// Writes the given list of tasks to the JSON file.
        /// Overwrites existing data.
        /// </summary>
        public void WriteTasks(List<TaskItem> tasks)
        {
            // Convert task list into formatted JSON (readable structure)
            var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(FilePath, json);
        }
    }
}
