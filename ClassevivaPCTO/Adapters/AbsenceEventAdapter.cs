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
                return CurrentObject.evtCode.ToString().GetLocalizedStr("sing") + "GenericEventOnDayPreposition".GetLocalizedStr() +
                       CurrentObject.evtDate.ToString("dd/MM/yyyy");
            }
        }

        public string JustifiedText
        {
            get
            {
                if (CurrentObject.isJustified)
                {
                    if (string.IsNullOrEmpty(CurrentObject.justifReasonCode))
                    {
                        return CurrentObject.justifReasonDesc;
                    }
                    else
                    {
                        return CurrentObject.justifReasonDesc + " (" + CurrentObject.justifReasonCode + ")";
                    }
                }

                return "AbsencesToBeJustifiedText".GetLocalizedStr();
            }
        }

        public AbsenceEventAdapter(AbsenceEvent ev)
        {
            CurrentObject = ev;
        }
    }
}