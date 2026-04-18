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
                            WriteError("Please provide a search keyword.");
                            return;
                        }

                        string keyword = args[1];
                        string? statusFilter = args.Length > 2 ? args[2] : null;

                        var results = _taskService.Search(keyword, statusFilter);

                        if (!results.Any())
                        {
                            WriteInfo("No matching tasks found.");
                            return;
                        }

                        PrintTasks(results);
                        break;

                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }
            catch 
            {
                Console.WriteLine("Invalid input. Please check arguments");
            }
        }

        private void PrintTasks(List<TaskItem> tasks)
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks found.");
                return;
            }

            const int idWidth = 5;
            const int descWidth = 30;
            const int statusWidth = 15;
            const int dateWidth = 18;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine(
                $"{"ID",-idWidth}| {"Description",-descWidth}| {"Status",-statusWidth}| {"Updated",-dateWidth}");
            Console.WriteLine(new string('-', idWidth + descWidth + statusWidth + dateWidth + 10));
            Console.ResetColor();

            foreach (var t in tasks)
            {
                Console.Write($"{t.Id,-idWidth}| ");
                Console.Write($"{FormatText(t.Description, descWidth)}| ");

                WriteStatus(t.Status, statusWidth);
                Console.Write("| ");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"{t.UpdatedAt:dd-MM-yyyy HH:mm}".PadRight(dateWidth));
                Console.ResetColor();
            }

            // Summary
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nTotal Tasks: {tasks.Count}");
            Console.ResetColor();

            Console.WriteLine();
        }

        private void WriteStatus(string status, int width)
        {
            switch (status)
            {
                case "done":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                case "in-progress":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }

            Console.Write($"{status.PadRight(width)}");
            Console.ResetColor();
        }

        private string FormatText(string text, int width)
        {
            if (text.Length > width)
                return text.Substring(0, width - 3) + "...";

            return text.PadRight(width);
        }

        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✘ {message}");
            Console.ResetColor();
            Console.WriteLine();
        }

        private void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"ℹ {message}");
            Console.ResetColor();
        }

    }
}
