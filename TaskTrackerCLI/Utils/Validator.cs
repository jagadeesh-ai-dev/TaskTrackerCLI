using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerCLI.Utils
{
    public static class Validator
    {
        public static bool IsValidId(string input, out int id)
        {
            return int.TryParse(input, out id);
        }
    }
}
