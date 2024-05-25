using ClassevivaPCTO.Utils;
using Windows.UI.Xaml;

namespace ClassevivaPCTO.Adapters
{
    public class NoteAdapter
    {
        public readonly Note CurrentObject;

        public NoteAdapter(Note ev)
        {
            CurrentObject = ev;
        }

        public Style ReadButtonStyle
        {
            get
            {
                return CurrentObject.readStatus
                    ? (Style) Application.Current.Resources["DefaultButtonStyle"]
                    : (Style) Application.Current.Resources["AccentButtonStyle"];
            }
        }
    }
}