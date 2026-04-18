using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                        _taskService.Delete(int.Parse(args[1]));
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

                        foreach(var task in tasks)
                        {
                            Console.WriteLine($"{task.Id} | {task.Description} | {task.Status}");
                        }
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


    }
}
