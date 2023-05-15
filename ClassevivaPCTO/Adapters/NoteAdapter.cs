using ClassevivaPCTO.Utils;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace ClassevivaPCTO.Adapters
{
    public class NoteAdapter
    {
        public readonly Note CurrentObject;

        public NoteAdapter(Note ev)
        {
            CurrentObject = ev;
        }
    }
}
