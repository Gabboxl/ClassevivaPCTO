using ClassevivaPCTO.Utils;
using System.Security.Policy;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace ClassevivaPCTO.Adapters
{
    public class NoticeAdapter
    {
        public readonly Notice CurrentObject;

        public string ReadIcon
        {
            get
            {
                if (CurrentObject.readStatus)
                {
                    return "\ue8c3"; //open mail

                }
                else
                {
                    return "\ue715"; //closed mail
                }
            }
        }

        public SolidColorBrush ReadColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();

                if (CurrentObject.readStatus)
                {
                    brush.Color = Colors.Green;

                }
                else
                {
                    brush.Color = Colors.Red;
                }

                return brush;
            }
        }

        public string FromToValidDate
        {
            get
            {
                return "valida dal " + CurrentObject.cntValidFrom.ToString("dd/MM/yyyy") + " al " + CurrentObject.cntValidTo.ToString("dd/MM/yyyy");
            }
            
        }

        public string Category
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentObject.cntCategory))
                {
                    return "";
                }
                else
                {
                    return CurrentObject.cntCategory;
                }
            }
        }


        public NoticeAdapter(Notice ev)
        {
            CurrentObject = ev;
        }
    }
}
