using ClassevivaPCTO.Helpers.Palettes;
using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using Windows.UI;
using Windows.UI.Xaml.Media;
using ClassevivaPCTO.Helpers;

namespace ClassevivaPCTO.Adapters
{
    public class NoticeAdapter
    {
        public readonly Notice CurrentObject;

        private IPalette _currentPalette = PaletteSelectorService.PaletteClass;

        public bool IsDeleted
        {
            get { return CurrentObject.cntStatus == "deleted"; }
        }

        public SolidColorBrush StatusTextColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();

                if (CurrentObject.cntStatus == "deleted")
                {
                    brush.Color = _currentPalette.ColorRed;
                }
                else if (!CurrentObject.cntValidInRange)
                {
                    brush.Color = _currentPalette.ColorOrange;
                }
                else
                {
                    brush.Color = _currentPalette.ColorGreen;
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
                    return "NoticeBoardDeletedStatusText".GetLocalizedStr();
                }
                else if (!CurrentObject.cntValidInRange)
                {
                    return "NoticeBoardExpiredStatusText".GetLocalizedStr();
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
                SolidColorBrush brush = new();

                if (CurrentObject.readStatus)
                {
                    brush.Color = _currentPalette.ColorGreen;
                }
                else
                {
                    brush.Color = _currentPalette.ColorRed;
                }

                return brush;
            }
        }

        public string FromToValidDate
        {
            get
            {
                return string.Format("NoticeBoardValidFromToDateText".GetLocalizedStr(),
                                     CurrentObject.cntValidFrom.ToString("dd/MM/yyyy"),CurrentObject.cntValidTo.ToString("dd/MM/yyyy"));
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