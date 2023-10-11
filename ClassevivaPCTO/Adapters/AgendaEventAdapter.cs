using ClassevivaPCTO.Utils;

namespace ClassevivaPCTO.Adapters
{
    public class AgendaEventAdapter
    {
        public readonly AgendaEvent CurrentObject;

        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentObject.subjectDesc))
                {
                    return CurrentObject.authorName;
                }
                else
                {
                    return CurrentObject.subjectDesc + " (" + CurrentObject.authorName + ")";
                }
            }
        }

        public string EventTypeIcon
        {
            get
            {
                return CurrentObject.evtCode switch
                {
                    "AGNT" => "\uE787",
                    "AGHW" => "\uEA98",
                    _ => "\uE787"
                };
            }
        }

        public string Notes => CurrentObject.notes;

        public AgendaEventAdapter(AgendaEvent ev)
        {
            CurrentObject = ev;
        }
    }
}