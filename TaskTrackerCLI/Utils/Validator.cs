using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerCLI.Utils
{
    /// <summary>
    /// Provides validation methods for user input.
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Validates whether the input is a valid positive integer ID.
        /// </summary>
        public static bool IsValidId(string input, out int id)
        {
            // Check if input is a number
            if (!int.TryParse(input, out id))
                return false;

            // Ensure ID is positive
            if (id <= 0)
                return false;

            return true;
        }
    }
}
