namespace ClassevivaPCTO.Utils
{
    public class AgendaEventAdapter : BindableBase.BindableBase
    {
        public AgendaEvent Event;

        public string Title { 
            get {
                if(string.IsNullOrEmpty(Event.subjectDesc))
                {
                    return Event.authorName;
                } else
                {
                    return Event.subjectDesc + " (" + Event.authorName + ")";
                }

            
            }
        }

        public string Notes => Event.notes;

        public AgendaEventAdapter(AgendaEvent ev)
        {
            Event = ev;
        }
    }

}
