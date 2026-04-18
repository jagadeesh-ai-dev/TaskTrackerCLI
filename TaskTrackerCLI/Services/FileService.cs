using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using TaskTrackerCLI.Models;

namespace TaskTrackerCLI.Services
{
    public class FileService
    {
        private readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "tasks.json");

        public List<TaskItem> ReadTasks()
        {
            if (!File.Exists(FilePath))
            {
                File.WriteAllText(FilePath, "[]");
            }
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<TaskItem>>(json)
                ?? new List<TaskItem>();
        }

        public void WriteTasks(List<TaskItem> tasks)
        {
            var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }
}
