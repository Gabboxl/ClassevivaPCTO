using ClassevivaPCTO.Utils;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace ClassevivaPCTO.Adapters
{
    public class ScrutinioAdapter
    {
        public readonly ScrutiniDocument CurrentObject;

        public SolidColorBrush StatusTextColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();

                if (CurrentObject.checkStatus.available)
                {
                    brush.Color = Colors.Green;
                }
                else
                {
                    brush.Color = Colors.IndianRed;
                }

                return brush;
            }
        }

        public string StatusText
        {
            get
            {
                if (CurrentObject.checkStatus.available)
                {
                    return " ";
                }
                else
                {
                    return "Non disponibile";
                }
            }
        }


        public ScrutinioAdapter(ScrutiniDocument ev)
        {
            CurrentObject = ev;
        }
    }
}
