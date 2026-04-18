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

        public TaskService()
        {
            _fileService = new FileService();
        }

        public void Add(string description)
        {
            var tasks = _fileService.ReadTasks();

            if (string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("Task description cannot be empty.");
                return;
            }

            description = description.Trim();
            bool isDuplicate = tasks.Any(t =>
                t.Description.Trim().Equals(description, StringComparison.OrdinalIgnoreCase));

            if (isDuplicate)
            {
                Console.WriteLine("Task already exists.");
                return;
            }

            int newId = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;

            var task = new TaskItem
            {
                Id = newId,
                Description = description,
                Status = "todo",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            tasks.Add(task);
            _fileService.WriteTasks(tasks);

            Console.WriteLine($"Task added successfully (ID: {newId})");
        }

        public void Update(int id, string description)
        {
            var tasks = _fileService.ReadTasks();

            var task = tasks.FirstOrDefault(t => t.Id == id);
            if(task == null)
            {
                Console.WriteLine("Task not found");
                return;
            }

            description = description.Trim();
            bool duplicate = tasks.Any(t =>
                t.Id != id &&
                t.Description.Trim().Equals(description, StringComparison.OrdinalIgnoreCase));

            if (duplicate)
            {
                Console.WriteLine("Another task with same description exists.");
                return;
            }

            task.Description = description;
            task.UpdatedAt = DateTime.Now;

            _fileService.WriteTasks(tasks);

            Console.WriteLine("Task updated successfully");
        }

        public void Delete (int id)
        {
            var tasks = _fileService.ReadTasks();

            var task = tasks.FirstOrDefault(t => t.Id == id);
            if(task == null)
            {
                Console.WriteLine("Task not found");
                return;
            }
            tasks.Remove(task);

            _fileService.WriteTasks(tasks);

            Console.WriteLine("Task deleted successfully");
        }

        public void MarkStatus (int id, string status)
        {
            var tasks = _fileService.ReadTasks();

            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                Console.WriteLine("Task not found");
                return;
            }

            task.Status = status;
            task.UpdatedAt = DateTime.Now;

            _fileService.WriteTasks(tasks);

            Console.WriteLine($"Task marked as {status}");
        }

        public List<TaskItem> GetAll (string? filter = null)
        {
            var tasks = _fileService.ReadTasks();
            return filter switch
            {
                "todo" => tasks.Where(t => t.Status == "todo").ToList(),
                "done" => tasks.Where(t => t.Status == "done").ToList(),
                "in-progress" => tasks.Where(t => t.Status == "in-progress").ToList(),
                _ => tasks
            };
        }

    }
}
