using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerCLI.Models;

namespace TaskTrackerCLI.Services
{
    /// <summary>
    /// Defines contract for managing tasks.
    /// Provides operations for creating, updating, deleting,
    /// searching, and retrieving tasks.
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// Adds a new task with given description.
        /// </summary>
        void Add(string description);

        /// <summary>
        /// Updates the description of an existing task.
        /// </summary>
        void Update(int id, string description);

        /// <summary>
        /// Deletes a task by ID.
        /// If force is false, confirmation may be required.
        /// </summary>
        void Delete(int id, bool force = false);

        /// <summary>
        /// Updates the status of a task (todo, in-progress, done).
        /// </summary>
        void MarkStatus(int id, string status);

        /// <summary>
        /// Retrieves all tasks, optionally filtered or sorted.
        /// </summary>
        List<TaskItem> GetAll(string? filter = null);

        /// <summary>
        /// Searches tasks by keyword and optional status.
        /// </summary>
        List<TaskItem> Search(string keyword, string? status = null);

        /// <summary>
        /// Retrieves a task by its ID.
        /// Returns null if not found.
        /// </summary>
        TaskItem? GetById(int id);
    }
}
