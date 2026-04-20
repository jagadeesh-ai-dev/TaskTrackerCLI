using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerCLI.Models;
using TaskTrackerCLI.Services;

namespace TaskTrackerCLI.Commands
{
    /// <summary>
    /// Handles user commands from CLI input and delegates actions to TaskService.
    /// Also responsible for formatting and displaying output to the console.
    /// </summary>
    public class CommandHandler
    {
        private readonly ITaskService _taskService;

        public CommandHandler()
        {
            // Initialize service (can be replaced with DI later)
            _taskService = new TaskService();
        }

        /// <summary>
        /// Entry point for handling CLI commands.
        /// Parses arguments and routes to appropriate action.
        /// </summary>
        public void Handle(string[] args)
        {
            // No command provided
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
                        // Support optional force flag to skip confirmation
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
                        // Optional filter (status or sorting)
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
                        // Validate ID input
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
                // Generic safeguard for invalid inputs (index out of range, parse errors, etc.)
                WriteError("Invalid input. Please check arguments");
            }
        }

        /// <summary>
        /// Displays tasks in a formatted table.
        /// Applies fixed-width columns for alignment.
        /// </summary>
        private void PrintTasks(List<TaskItem> tasks)
        {
            if (tasks.Count == 0)
            {
                WriteInfo("No tasks found.\n");
                return;
            }

            // Column widths (adjust carefully to maintain alignment)
            const int idWidth = 5;
            const int descWidth = 30;
            const int statusWidth = 15;
            const int createdDateWidth = 22;
            const int updatedDateWidth = 22;

            // Header
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine(
                $"{"ID",-idWidth} | {"Description",-descWidth} | {"Status",-statusWidth} | {"Created",-createdDateWidth} | {"Updated",-updatedDateWidth}");

            // Divider line based on total width
            int totalWidth =
                idWidth + descWidth + statusWidth + createdDateWidth + updatedDateWidth
                + (4 * 3); // 4 separators " | "

            Console.WriteLine(new string('-', totalWidth));
            Console.ResetColor();

            foreach (var t in tasks)
            {
                Console.Write($"{t.Id,-idWidth} | ");

                // Truncate long descriptions to maintain table layout
                Console.Write($"{FormatText(t.Description, descWidth)} | ");

                // Colored status
                WriteStatus(t.Status, statusWidth);

                Console.Write(" | ");

                Console.ForegroundColor = ConsoleColor.DarkGray;

                // Convert UTC to local time for user readability
                Console.Write($"{t.CreatedAt.ToLocalTime():dd-MM-yyyy hh:mm tt}".PadRight(createdDateWidth));
                Console.Write(" | ");

                // Show dash if task was never updated
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

        /// <summary>
        /// Displays full details of a single task (no truncation).
        /// </summary>
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

            // Display friendly message if not updated yet
            Console.WriteLine($"Updated     : {(t.UpdatedAt.HasValue
                ? t.UpdatedAt.Value.ToLocalTime().ToString("dd-MM-yyyy hh:mm tt")
                : "Not updated yet")}");

            Console.ResetColor();

            Console.WriteLine();
        }

        /// <summary>
        /// Returns console color based on task status.
        /// Ensures consistent color usage across the application.
        /// </summary>
        private ConsoleColor GetStatusColor(string status)
        {
            return status switch
            {
                "done" => ConsoleColor.Green,
                "in-progress" => ConsoleColor.Yellow,
                _ => ConsoleColor.Gray
            };
        }

        /// <summary>
        /// Writes status with padding (used in table view).
        /// </summary>
        private void WriteStatus(string status, int width)
        {
            Console.ForegroundColor = GetStatusColor(status);
            Console.Write(status.PadRight(width));
            Console.ResetColor();
        }

        /// <summary>
        /// Writes status inline without padding (used in detail view).
        /// </summary>
        private void WriteStatusInline(string status)
        {
            Console.ForegroundColor = GetStatusColor(status);
            Console.Write(status);
            Console.ResetColor();
        }

        /// <summary>
        /// Truncates long text and adds ellipsis to maintain table layout.
        /// </summary>
        private string FormatText(string text, int width)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "".PadRight(width);

            return text.Length > width
                ? text.Substring(0, width - 3) + "..."
                : text.PadRight(width);
        }

        /// <summary>
        /// Displays error messages in red.
        /// </summary>
        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERR] {message}");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Displays informational messages in cyan.
        /// </summary>
        private void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[INF] {message}");
            Console.ResetColor();
        }
    }
}
