using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace ClassevivaPCTO.Utils
{
    public class CvUtils
    {
        public string GetCode(string userId)
        {
            return Regex.Replace(userId, @"[A-Za-z]+", "");

        }


        public static SolidColorBrush GetColorFromAbsenceCode(AbsenceEventCode valore)
        {
            SolidColorBrush brush = new SolidColorBrush();


            switch (valore)
            {
                case AbsenceEventCode.ABA0:
                    brush.Color = Colors.Crimson;
                    break;

                case AbsenceEventCode.ABR0:
                    brush.Color = Colors.DarkOrange;
                    break;

                case AbsenceEventCode.ABR1:
                    brush.Color = Colors.DarkOrange;
                    break;

                case AbsenceEventCode.ABU0:
                    brush.Color = Colors.Goldenrod;
                    break;
            }

            return brush;
        }
    }

}
