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
        public static void Main(string[] args)
        {
            try
            {
                if(args.Length == 0)
                {
                    ShowHelp();
                    return;
                }

                var handler = new CommandHandler();
                handler.Handle(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong. Please check your command and try again.");
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Task Tracker CLI Usage:");
            Console.WriteLine();

            Console.WriteLine("Add a task:");
            Console.WriteLine(" add \"Task description\"");

            Console.WriteLine("\nUpdate a task:");
            Console.WriteLine(" update \"New description\"");

            Console.WriteLine("\nDelete a task:");
            Console.WriteLine(" delete <id>");

            Console.WriteLine("\nMark task status:");
            Console.WriteLine(" mark-in-progress <id>");
            Console.WriteLine(" mark-done <id>");

            Console.WriteLine("\nList tasks:");
            Console.WriteLine(" list");
            Console.WriteLine(" list todo");
            Console.WriteLine(" list in-progress");
            Console.WriteLine(" list done");


            Console.WriteLine("\n Examples:");
            Console.WriteLine(" add \"Buy groceries\"");
            Console.WriteLine(" update 1 \"Buy groceries and cook dinner\"");
            Console.WriteLine(" delete 1");
            Console.WriteLine(" mark-done 1");
            Console.WriteLine(" list done");

        }
    }
}
