using ClassevivaPCTO.Utils;

namespace ClassevivaPCTO.Adapters
{
    public class AbsenceEventAdapter
    {
        public readonly AbsenceEvent CurrentObject;

        public string JustifiedText
        {
            get
            {
                if ((bool)CurrentObject.isJustified)
                {
                    return CurrentObject.justifReasonDesc + " (" + CurrentObject.justifReasonCode +")";
                }
                else
                {
                    return "Da giustificare";
                }
            }
        }

        public AbsenceEventAdapter(AbsenceEvent ev)
        {
            CurrentObject = ev;
        }
    }
}
