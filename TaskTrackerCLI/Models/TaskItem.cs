using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerCLI.Models
{
    /// <summary>
    /// Represents a task entity in the system.
    /// Stores task details, status, and timestamps.
    /// </summary>
    public class TaskItem
    {
        /// <summary>
        /// Unique identifier for the task.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Short description of the task.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the task (todo, in-progress, done).
        /// </summary>
        public string Status { get; set; } = "todo";

        /// <summary>
        /// Timestamp when the task was created (stored in UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when the task was last updated (nullable).
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
