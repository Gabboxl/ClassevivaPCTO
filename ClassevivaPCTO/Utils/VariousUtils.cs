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


        public static string ToApiDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }

        public static float? GradeToFloat(object value)
        {
            float? valoreFinale = null;

            if (value is Grade)
            {
                Grade grade = (Grade)value;

                if (grade.decimalValue != null)
                {
                    valoreFinale = grade.decimalValue;
                }
                else if (!grade.displayValue.ToLower().Equals("nv"))
                {

                    switch (grade.displayValue)
                    {
                        case "o":
                            valoreFinale = 10;
                            break;

                        case "ds":
                            valoreFinale = 9;
                            break;

                        case "b":
                            valoreFinale = 8;
                            break;

                        case "dc":
                            valoreFinale = 7;
                            break;

                        case "s":
                            valoreFinale = 6;
                            break;

                        case "i":
                            valoreFinale = 5;
                            break;
                    }
                }
            }
            else
            {
                valoreFinale = (float)value;
            }


            return valoreFinale;
        }
    }

}
