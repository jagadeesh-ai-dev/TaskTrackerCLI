using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerCLI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "todo";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
