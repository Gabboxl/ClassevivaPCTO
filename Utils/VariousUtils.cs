using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassevivaPCTO.Utils
{
    internal class VariousUtils
    {
        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            s = s.ToLower();
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
