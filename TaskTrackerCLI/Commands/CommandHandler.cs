using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerCLI.Models;
using TaskTrackerCLI.Services;

namespace TaskTrackerCLI.Commands
{
    public class CommandHandler
    {
        private readonly ITaskService _taskService;

        public CommandHandler()
        {
            _taskService = new TaskService();
        }

        public void Handle(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No command provided");
                return;
            }

            try
            {
                switch (args[0])
                {
                    case "add":
                        _taskService.Add(args[1]);
                        break;

                    case "update":
                        _taskService.Update(int.Parse(args[1]), args[2]);
                        break;

                    case "delete":
                        bool force = args.Contains("--force");
                        _taskService.Delete(int.Parse(args[1]), force);
                        break;

                    case "mark-in-progress":
                        _taskService.MarkStatus(int.Parse(args[1]), "in-progress");
                        break;

                    case "mark-done":
                        _taskService.MarkStatus(int.Parse(args[1]), "done");
                        break;

                    case "list":
                        var filter = args.Length > 1 ? args[1] : null;
                        
                        var tasks = _taskService.GetAll(filter);

                        PrintTasks(tasks);
                        break;

                    case "search":
                        if (args.Length < 2)
                        {
                            WriteError("Please provide a search keyword.\n");
                            return;
                        }

                        string keyword = args[1];
                        string? statusFilter = args.Length > 2 ? args[2] : null;

                        var results = _taskService.Search(keyword, statusFilter);

                        if (!results.Any())
                        {
                            WriteInfo("No matching tasks found.\n");
                            return;
                        }

                        PrintTasks(results);
                        break;

                    case "view":
                        if (args.Length < 2 || !int.TryParse(args[1], out int viewId))
                        {
                            WriteError("Please provide a valid task ID.");
                            return;
                        }

                        var task = _taskService.GetById(viewId);

                        if (task == null)
                        {
                            WriteError("Task not found.");
                            return;
                        }

                        PrintTaskDetails(task);
                        break;

                    default:
                        WriteError("Invalid command");
                        break;
                }
            }
            catch 
            {
                WriteError("Invalid input. Please check arguments");
            }
        }

        private void PrintTasks(List<TaskItem> tasks)
        {
            if (tasks.Count == 0)
            {
                WriteInfo("No tasks found.\n");
                return;
            }

            const int idWidth = 5;
            const int descWidth = 30;
            const int statusWidth = 15;
            const int createdDateWidth = 22;  
            const int updatedDateWidth = 22;  

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine(
                $"{"ID",-idWidth} | {"Description",-descWidth} | {"Status",-statusWidth} | {"Created",-createdDateWidth} | {"Updated",-updatedDateWidth}");
            int totalWidth =
                idWidth + descWidth + statusWidth + createdDateWidth + updatedDateWidth
                + (5 * 3); // 5 columns -> 4 separators " | "

            Console.WriteLine(new string('-', totalWidth));
            Console.ResetColor();

            foreach (var t in tasks)
            {
                Console.Write($"{t.Id,-idWidth} | ");
                Console.Write($"{FormatText(t.Description, descWidth)} | ");

                WriteStatus(t.Status, statusWidth);

                Console.Write(" | ");

                Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.Write($"{t.CreatedAt.ToLocalTime():dd-MM-yyyy hh:mm tt}".PadRight(createdDateWidth));
                Console.ResetColor();
                Console.Write(" | ");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(
                    (t.UpdatedAt.HasValue
                        ? t.UpdatedAt.Value.ToLocalTime().ToString("dd-MM-yyyy hh:mm tt")
                        : "—"
                    ).PadRight(updatedDateWidth)
                );

                Console.ResetColor();
            }

            // Summary
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nTotal Tasks: {tasks.Count}");
            Console.ResetColor();

            Console.WriteLine();
        }

        private void PrintTaskDetails(TaskItem t)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nTASK DETAILS\n");
            Console.ResetColor();

            Console.WriteLine($"ID          : {t.Id}");
            Console.WriteLine($"Description : {t.Description}");

            Console.Write("Status      : ");
            WriteStatusInline(t.Status);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Created     : {t.CreatedAt.ToLocalTime():dd-MM-yyyy hh:mm tt}");
            Console.WriteLine($"Updated     : {(t.UpdatedAt.HasValue
                ? t.UpdatedAt.Value.ToLocalTime().ToString("dd-MM-yyyy hh:mm tt")
                : "Not updated yet")}");
            Console.ResetColor();

            Console.WriteLine();
        }

        private ConsoleColor GetStatusColor(string status)
        {
            return status switch
            {
                "done" => ConsoleColor.Green,
                "in-progress" => ConsoleColor.Yellow,
                _ => ConsoleColor.Gray
            };
        }

        private void WriteStatus(string status, int width)
        {
            Console.ForegroundColor = GetStatusColor(status);
            Console.Write(status.PadRight(width));
            Console.ResetColor();
        }

        private void WriteStatusInline(string status)
        {
            Console.ForegroundColor = GetStatusColor(status);
            Console.Write(status);
            Console.ResetColor();
        }

        private string FormatText(string text, int width)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "".PadRight(width);

            return text.Length > width
                ? text.Substring(0, width - 3) + "..."
                : text.PadRight(width);
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
