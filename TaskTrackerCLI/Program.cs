using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerCLI.Commands;

namespace TaskTrackerCLI
{

    /// <summary>
    /// Entry point of the application.
    /// Supports both direct command execution and interactive CLI mode.
    /// </summary>
    public class Program
    {
        // Stores command history for ↑ ↓ navigation
        private static List<string> _history = new();

        // Tracks current position in history
        private static int _historyIndex = -1;

        public static void Main(string[] args)
        {
            try
            {
                // If no arguments passed → start interactive CLI
                if (args.Length == 0)
                {
                    RunInteractiveMode();
                    return;
                }

                // Enable Unicode support (✔ ✘ symbols)
                Console.OutputEncoding = System.Text.Encoding.UTF8;

                var handler = new CommandHandler();
                handler.Handle(args);
            }
            catch (Exception ex)
            {
                // Global fallback for unexpected errors
                WriteError($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Runs interactive CLI loop where user can type commands continuously.
        /// Supports history navigation and command parsing.
        /// </summary>
        private static void RunInteractiveMode()
        {
            var handler = new CommandHandler();

            WriteInfo("Task Tracker CLI (Interactive Mode)");
            WriteInfo("Type 'help' for commands | 'exit' to quit\n");

            while (true)
            {
                // Prompt
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("> ");
                Console.ResetColor();

                var input = ReadLineWithHistory();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                // Save command to history
                _history.Add(input);
                _historyIndex = _history.Count;

                // Exit command
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                // Clear screen command
                if (input.Equals("clear", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Clear();
                    continue;
                }

                // Help menu
                if (input.Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    ShowHelp();
                    continue;
                }

                // Parse command (supports quoted input)
                var args = ParseArguments(input);

                try
                {
                    handler.Handle(args);
                }
                catch (Exception ex)
                {
                    WriteError($"Invalid command: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Reads user input with command history support (↑ ↓ navigation).
        /// Mimics basic shell behavior.
        /// </summary>
        private static string ReadLineWithHistory()
        {
            var input = "";
            int cursor = 0;

            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input;
                }

                // Handle backspace (remove last character)
                else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                    cursor--;

                    Console.Write("\b \b");
                }

                // Navigate up in history
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    if (_history.Count == 0) continue;

                    _historyIndex = Math.Max(0, _historyIndex - 1);
                    ReplaceLine(_history[_historyIndex], ref input);
                }

                // Navigate down in history
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (_history.Count == 0) continue;

                    _historyIndex = Math.Min(_history.Count, _historyIndex + 1);

                    var newValue = _historyIndex == _history.Count ? "" : _history[_historyIndex];
                    ReplaceLine(newValue, ref input);
                }

                // Regular character input
                else if (!char.IsControl(key.KeyChar))
                {
                    input += key.KeyChar;
                    cursor++;
                    Console.Write(key.KeyChar);
                }
            }
        }

        /// <summary>
        /// Replaces current console input line (used for history navigation).
        /// </summary>
        private static void ReplaceLine(string newValue, ref string input)
        {
            Console.Write("\r> ");
            Console.Write(new string(' ', Console.WindowWidth - 2));
            Console.Write("\r> ");

            input = newValue;
            Console.Write(input);
        }

        // <summary>
        /// Parses command string into arguments.
        /// Supports quoted strings (e.g., "buy groceries").
        /// </summary>
        private static string[] ParseArguments(string input)
        {
            var result = new List<string>();
            var current = "";
            bool inQuotes = false;

            foreach (var ch in input)
            {
                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (ch == ' ' && !inQuotes)
                {
                    if (!string.IsNullOrEmpty(current))
                    {
                        result.Add(current);
                        current = "";
                    }
                }
                else
                {
                    current += ch;
                }
            }

            if (!string.IsNullOrEmpty(current))
                result.Add(current);

            return result.ToArray();
        }

        /// <summary>
        /// Displays available commands and usage examples.
        /// </summary>
        private static void ShowHelp()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nTASK TRACKER COMMANDS\n");
            Console.ResetColor();

            // 🔹 Create / Update
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("CREATE & UPDATE");
            Console.ResetColor();
            Console.WriteLine("  add \"task description\"        -> add a new task");
            Console.WriteLine("  update <id> \"new description\" -> update existing task\n");

            // 🔹 Delete
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("DELETE");
            Console.ResetColor();
            Console.WriteLine("  delete <id> --force                 -> delete a task by ID (Confirmation not required)\n");
            Console.WriteLine("  delete <id>                         -> delete a task by ID (Confirmation required\n");

            // 🔹 Status
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("STATUS");
            Console.ResetColor();
            Console.WriteLine("  mark-in-progress <id>        -> mark task as in progress");
            Console.WriteLine("  mark-done <id>               -> mark task as completed\n");

            // 🔹 Listing
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("VIEW TASKS");
            Console.ResetColor();
            Console.WriteLine("  list                     -> show all tasks");
            Console.WriteLine("  list done                -> show completed tasks");
            Console.WriteLine("  list todo                -> show pending tasks");
            Console.WriteLine("  list in-progress         -> show active tasks");
            Console.WriteLine("  list newest              -> sort by latest created");
            Console.WriteLine("  list oldest              -> sort by earliest created");
            Console.WriteLine("  list recent              -> sort by last updated");
            Console.WriteLine("  list stale               -> sort by least recently updated\n");

            // 🔹 Searching
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("SEARCH TASKS");
            Console.ResetColor();
            Console.WriteLine("  search \"keyword\"                -> search by description");
            Console.WriteLine("  search \"keyword\" done           -> search completed tasks");
            Console.WriteLine("  search \"keyword\" in-progress    -> search in-progress tasks");
            Console.WriteLine("  search \"keyword\" todo           -> search pending tasks\n");

            // 🔹 Utility
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("UTILITY");
            Console.ResetColor();
            Console.WriteLine("  clear   -> clear screen");
            Console.WriteLine("  exit    -> close app\n");

            // 🔹 View
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("View");
            Console.WriteLine("  view <id>                -> show full task details");

            // 🔹 Example
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("EXAMPLES");
            Console.ResetColor();
            Console.WriteLine("  add \"Buy groceries\"");
            Console.WriteLine("  update 1 \"Buy milk\"");
            Console.WriteLine("  mark-done 1");
            Console.WriteLine("  view 1");
            Console.WriteLine("  list newest");
            Console.WriteLine("  list done\n");
        }

        /// <summary>
        /// Writes informational message.
        /// </summary>
        private static void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Writes error message.
        /// </summary>
        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERR] {message}");
            Console.ResetColor();
        }
    }
}
