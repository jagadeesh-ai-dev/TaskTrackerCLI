using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerCLI.Models;

namespace TaskTrackerCLI.Services
{
    public interface ITaskService
    {
        void Add(string description);
        void Update(int id, string description);
        void Delete(int id, bool force = false);
        void MarkStatus(int id, string status);
        List<TaskItem> GetAll(string? filter = null);
        List<TaskItem> Search(string keyword, string? status = null);
        TaskItem? GetById(int id);
    }
}
