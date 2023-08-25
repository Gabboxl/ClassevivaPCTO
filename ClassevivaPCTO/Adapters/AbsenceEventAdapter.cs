using ClassevivaPCTO.Utils;

namespace ClassevivaPCTO.Adapters
{
    public class AbsenceEventAdapter
    {
        public readonly AbsenceEvent CurrentObject;

        public string ShortEventName
        {
            get { return CurrentObject.evtCode.GetShortName(); }
        }

        public string EventTitle
        {
            get { return CurrentObject.evtCode.GetLongName() + " del " + CurrentObject.evtDate.ToString("dd/MM/yyyy"); }
        }

        public string JustifiedText
        {
            get
            {
                if ((bool) CurrentObject.isJustified)
                {
                    return CurrentObject.justifReasonDesc + " (" + CurrentObject.justifReasonCode + ")";
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