using ClassevivaPCTO.Helpers;
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
            get
            {
                return CurrentObject.evtCode.ToString().GetLocalized("sing") +
                       CurrentObject.evtDate.ToString("dd/MM/yyyy");
            }
        }

        public string JustifiedText
        {
            get
            {
                if (CurrentObject.isJustified)
                {
                    return CurrentObject.justifReasonDesc + " (" + CurrentObject.justifReasonCode + ")";
                }
                else
                {
                    return "ToBeJustifiedText".GetLocalized();
                }
            }
        }

        public AbsenceEventAdapter(AbsenceEvent ev)
        {
            CurrentObject = ev;
        }
    }
}