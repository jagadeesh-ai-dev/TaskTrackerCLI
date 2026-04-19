using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerCLI.Commands;

namespace TaskTrackerCLI
{


    public class Program
    {
        private static List<string> _history = new();
        private static int _historyIndex = -1;

        public static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    RunInteractiveMode();
                    return;
                }

                Console.OutputEncoding = System.Text.Encoding.UTF8;

                var handler = new CommandHandler();
                handler.Handle(args);
            }
            catch (Exception ex)
            {
                WriteError($"Error: {ex.Message}");
            }
        }

        // INTERACTIVE MODE
        private static void RunInteractiveMode()
        {
            var handler = new CommandHandler();

            WriteInfo("Task Tracker CLI (Interactive Mode)");
            WriteInfo("Type 'help' for commands | 'exit' to quit\n");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("> ");
                Console.ResetColor();

                var input = ReadLineWithHistory();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                _history.Add(input);
                _historyIndex = _history.Count;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (input.Equals("clear", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Clear();
                    continue;
                }

                if (input.Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    ShowHelp();
                    continue;
                }

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

        // COMMAND HISTORY (↑ ↓ SUPPORT)
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

                else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                    cursor--;

                    Console.Write("\b \b");
                }

                else if (key.Key == ConsoleKey.UpArrow)
                {
                    if (_history.Count == 0) continue;

                    _historyIndex = Math.Max(0, _historyIndex - 1);
                    ReplaceLine(_history[_historyIndex], ref input);
                }

                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (_history.Count == 0) continue;

                    _historyIndex = Math.Min(_history.Count, _historyIndex + 1);

                    var newValue = _historyIndex == _history.Count ? "" : _history[_historyIndex];
                    ReplaceLine(newValue, ref input);
                }

                else if (!char.IsControl(key.KeyChar))
                {
                    input += key.KeyChar;
                    cursor++;
                    Console.Write(key.KeyChar);
                }
            }
        }

        private static void ReplaceLine(string newValue, ref string input)
        {
            // Clear current line
            Console.Write("\r> ");
            Console.Write(new string(' ', Console.WindowWidth - 2));
            Console.Write("\r> ");

            input = newValue;
            Console.Write(input);
        }

        // ARGUMENT PARSER (Handles quotes)
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

        // HELP MENU
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
            Console.WriteLine("  delete <id>                  -> delete a task by ID\n");

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

        // COLORED OUTPUT HELPERS
        private static void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] {message}");
            Console.ResetColor();
        }

        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERR] {message}");
            Console.ResetColor();
        }
    }
}
