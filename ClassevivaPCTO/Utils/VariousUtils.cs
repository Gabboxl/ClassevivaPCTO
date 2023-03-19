using Newtonsoft.Json.Linq;
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

        public static float? GradeToInt(object value)
        {
            float? valore = null;

            if (value is Grade)
            {
                Grade grade = (Grade)value;

                if (grade.decimalValue != null)
                {
                    valore = grade.decimalValue;
                }
                else if (!grade.displayValue.ToLower().Equals("nv"))
                {

                    switch (grade.displayValue)
                    {
                        case "o":
                            valore = 10;
                            break;

                        case "ds":
                            valore = 9;
                            break;

                        case "b":
                            valore = 8;
                            break;

                        case "dc":
                            valore = 7;
                            break;

                        case "s":
                            valore = 6;
                            break;

                        case "i":
                            valore = 5;
                            break;
                    }
                }
            }
            else
            {
                valore = (float?)value;
            }





            return valore;
        }
    }

}
