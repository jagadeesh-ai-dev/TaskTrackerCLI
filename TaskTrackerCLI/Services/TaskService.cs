using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerCLI.Models;

namespace TaskTrackerCLI.Services
{
    public class TaskService : ITaskService
    {
        private readonly FileService _fileService;
        private static readonly string[] ValidStatuses = { "todo", "in-progress", "done" };

        public TaskService()
        {
            _fileService = new FileService();
        }

        public void Add(string description)
        {
            var tasks = _fileService.ReadTasks();
            
            description = Normalize(description);

            if (string.IsNullOrEmpty(description))
            {
                WriteError("Task description cannot be empty.\n");
                return;
            }

            if (IsDuplicate(tasks, description))
            {
                WriteError("Task already exists.\n");
                return;
            }

            int newId = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;

            tasks.Add(new TaskItem
            { 
                Id = newId,
                Description = description,
                Status = "todo",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            _fileService.WriteTasks(tasks);

            WriteSuccess($"Task added (ID: {newId}).\n");
        }

        public void Update(int id, string description)
        {
            var tasks = _fileService.ReadTasks();
            var task = tasks.FirstOrDefault(t => t.Id == id);

            if(task == null)
            {
                WriteInfo("Task not found.\n");
                return;
            }

            description = Normalize(description);

            if (IsDuplicate(tasks, description, id))
            {
                WriteError("Another task with same description exists.\n");
                return;
            }
            var taskName = task.Description;
            task.Description = description;
            task.UpdatedAt = DateTime.UtcNow;

            _fileService.WriteTasks(tasks);

            WriteSuccess("Task updated.\n");
        }

        public void Delete(int id, bool force = false)
        {
            var tasks = _fileService.ReadTasks();
            var task = tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                WriteInfo("Task not found.\n");
                return;
            }

            if (!force)
            {
                Console.Write($"Delete '{task.Description}'? (y/n): ");
                var confirm = Console.ReadLine()?.Trim().ToLower();

                if (confirm != "y")
                {
                    WriteInfo("\nDelete cancelled.\n");
                    return;
                }
            }

            tasks.Remove(task);
            _fileService.WriteTasks(tasks);

            WriteSuccess("Task deleted.\n");
        }

        public void MarkStatus (int id, string status)
        {
            var tasks = _fileService.ReadTasks();
            var task = tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                WriteInfo("Task not found.\n");
                return;
            }

            status = NormalizeStatus(status);

            if (!ValidStatuses.Contains(status))
            {
                WriteError("Invalid status.\n");
                return;
            }

            if (task.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
            {
                WriteInfo($"Task is already marked as '{status}'.\n");
                return;
            }

            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;

            _fileService.WriteTasks(tasks);

            WriteSuccess($"Task marked as {status}\n");
        }

        public List<TaskItem> GetAll (string? filter = null)
        {
            var tasks = _fileService.ReadTasks();
            return filter switch
            {
                "todo" or "t" => tasks.Where(t => t.Status == "todo").ToList(),
                "done" or "completed" or "d" => tasks.Where(t => t.Status == "done").ToList(),
                "in-progress" or "inprogress" or "progress" or "p" or "ip" => tasks.Where(t => t.Status == "in-progress").ToList(),
                _ => tasks
            };
        }

        public List<TaskItem> Search(string keyword, string? status = null)
        {
            var tasks = _fileService.ReadTasks();

            keyword = keyword.Trim();

            var result = tasks.Where(t =>
                t.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                t.Status.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            );

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = NormalizeStatus(status);
                result = result.Where(t => t.Status == status);
            }

            return result.ToList();
        }

        private static string Normalize(string input)
        {
            return input?.Trim() ?? "";
        }

        private string NormalizeStatus(string status)
        {
            return status.ToLower() switch
            {
                "ip" or "progress" or "inprogress" => "in-progress",
                "d" or "done" => "done",
                "todo" or "t" => "todo",
                _ => status
            };
        }

        private static bool IsDuplicate(List<TaskItem> tasks, string description, int? excludeId = null)
        {
            return tasks.Any(t =>
                t.Id != excludeId &&
                t.Description.Trim().Equals(description, StringComparison.OrdinalIgnoreCase));
        }

        private static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] {message}");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERR] {message}");
            Console.ResetColor();
            Console.WriteLine();
        }

        private void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[INF] {message}");
            Console.ResetColor();
        }

    }
}
