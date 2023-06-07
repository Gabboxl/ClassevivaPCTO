using ClassevivaPCTO.Utils;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace ClassevivaPCTO.Adapters
{
    public class NoticeAdapter
    {
        public readonly Notice CurrentObject;

        public bool IsDeleted
        {
            get
            {
                return CurrentObject.cntStatus == "deleted";
            }
        }

        public SolidColorBrush StatusTextColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();

                if (CurrentObject.cntStatus == "deleted")
                {
                    brush.Color = Colors.Crimson;
                }
                else if (!CurrentObject.cntValidInRange)
                {
                    brush.Color = Colors.DarkOrange;
                }
                else
                {
                    brush.Color = Colors.Green;
                }

                return brush;
            }
        }

        public string StatusText
        {
            get
            {
                if (CurrentObject.cntStatus == "deleted")
                {
                    return "Comunicazione eliminata";
                } else if (!CurrentObject.cntValidInRange)
                {
                    return "Comunicazione scaduta";
                }

                return "";
            }
        }

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
                return "Valida dal " + CurrentObject.cntValidFrom.ToString("dd/MM/yyyy") + " al " + CurrentObject.cntValidTo.ToString("dd/MM/yyyy");
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
                    return "[" + CurrentObject.cntCategory + "]";
                }
            }
        }


        public NoticeAdapter(Notice ev)
        {
            CurrentObject = ev;
        }
    }
}
