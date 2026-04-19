using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerCLI.Models;

namespace TaskTrackerCLI.Services
{
    /// <summary>
    /// Handles all business logic related to task management.
    /// Responsible for validation, processing, and coordination with FileService.
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly FileService _fileService;

        /// <summary>
        /// Allowed task statuses to maintain data integrity.
        /// </summary>
        private static readonly string[] ValidStatuses = { "todo", "in-progress", "done" };

        public TaskService()
        {
            _fileService = new FileService();
        }

        /// <summary>
        /// Adds a new task after validating input and checking duplicates.
        /// </summary>
        public void Add(string description)
        {
            var tasks = _fileService.ReadTasks();

            // Normalize input (trim spaces)
            description = Normalize(description);

            // Validate empty input
            if (string.IsNullOrEmpty(description))
            {
                WriteError("Task description cannot be empty.\n");
                return;
            }

            // Prevent duplicate task descriptions (case-insensitive)
            if (IsDuplicate(tasks, description))
            {
                WriteError("Task already exists.\n");
                return;
            }

            // Generate unique ID
            int newId = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;

            // Create new task
            tasks.Add(new TaskItem
            {
                Id = newId,
                Description = description,
                Status = "todo",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null // No updates yet
            });

            _fileService.WriteTasks(tasks);

            WriteSuccess($"Task added (ID: {newId}).\n");
        }

        /// <summary>
        /// Updates an existing task description.
        /// Prevents duplicate and unnecessary updates.
        /// </summary>
        public void Update(int id, string description)
        {
            var tasks = _fileService.ReadTasks();
            var task = tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                WriteInfo("Task not found.\n");
                return;
            }

            description = Normalize(description);

            // Check if no actual change
            if (task.Description.Equals(description, StringComparison.OrdinalIgnoreCase))
            {
                WriteInfo("No changes detected. Task already has this description.");
                Console.WriteLine();
                return;
            }

            // Prevent duplicate descriptions across tasks
            if (IsDuplicate(tasks, description, id))
            {
                WriteError("Another task with same description exists.\n");
                return;
            }

            task.Description = description;
            task.UpdatedAt = DateTime.UtcNow;

            _fileService.WriteTasks(tasks);

            WriteSuccess("Task updated.\n");
        }

        /// <summary>
        /// Deletes a task by ID.
        /// Supports confirmation prompt unless forced.
        /// </summary>
        public void Delete(int id, bool force = false)
        {
            var tasks = _fileService.ReadTasks();
            var task = tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                WriteInfo("Task not found.\n");
                return;
            }

            // Ask confirmation unless forced
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

        /// <summary>
        /// Updates the status of a task (todo, in-progress, done).
        /// Includes validation and avoids redundant updates.
        /// </summary>
        public void MarkStatus(int id, string status)
        {
            var tasks = _fileService.ReadTasks();
            var task = tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                WriteInfo("Task not found.\n");
                return;
            }

            // Normalize user input (aliases like "ip" → "in-progress")
            status = NormalizeStatus(status);

            // Validate status value
            if (!ValidStatuses.Contains(status))
            {
                WriteError("Invalid status.\n");
                return;
            }

            // Avoid unnecessary updates
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

        /// <summary>
        /// Returns tasks based on filter or sorting criteria.
        /// Supports status filtering and basic sorting.
        /// </summary>
        public List<TaskItem> GetAll(string? filter = null)
        {
            var tasks = _fileService.ReadTasks();

            return filter switch
            {
                // Status filters
                "todo" or "t" => tasks.Where(t => t.Status == "todo").ToList(),
                "done" or "completed" or "d" => tasks.Where(t => t.Status == "done").ToList(),
                "in-progress" or "inprogress" or "progress" or "p" or "ip" => tasks.Where(t => t.Status == "in-progress").ToList(),

                // Sorting by creation time
                "newest" => tasks.OrderByDescending(t => t.CreatedAt).ToList(),
                "oldest" => tasks.OrderBy(t => t.CreatedAt).ToList(),

                // Sorting by update time
                "recent" => tasks.OrderByDescending(t => t.UpdatedAt).ToList(),
                "stale" => tasks.OrderBy(t => t.UpdatedAt).ToList(),

                _ => tasks
            };
        }

        /// <summary>
        /// Searches tasks by keyword in description or status.
        /// Optionally filters by status.
        /// </summary>
        public List<TaskItem> Search(string keyword, string? status = null)
        {
            var tasks = _fileService.ReadTasks();

            keyword = keyword.Trim();

            var result = tasks.Where(t =>
                t.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                t.Status.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            );

            // Optional status filter
            if (!string.IsNullOrWhiteSpace(status))
            {
                status = NormalizeStatus(status);
                result = result.Where(t => t.Status == status);
            }

            return result.ToList();
        }

        /// <summary>
        /// Returns a single task by ID.
        /// </summary>
        public TaskItem? GetById(int id)
        {
            var tasks = _fileService.ReadTasks();
            return tasks.FirstOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// Trims and normalizes input string.
        /// </summary>
        private static string Normalize(string input)
        {
            return input?.Trim() ?? "";
        }

        /// <summary>
        /// Converts user-friendly status inputs into standard values.
        /// </summary>
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

        /// <summary>
        /// Checks if a task description already exists (case-insensitive).
        /// Excludes a specific ID during update.
        /// </summary>
        private static bool IsDuplicate(List<TaskItem> tasks, string description, int? excludeId = null)
        {
            return tasks.Any(t =>
                t.Id != excludeId &&
                t.Description.Trim().Equals(description, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Prints success message in green.
        /// </summary>
        private static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] {message}");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Prints error message in red.
        /// </summary>
        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERR] {message}");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Prints informational message in cyan.
        /// </summary>
        private void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[INF] {message}");
            Console.ResetColor();
        }
    }
}
