namespace ClassevivaPCTO.Utils
{
    public class AgendaEventAdapter : BindableBase.BindableBase
    {
        private AgendaEvent _event;

        public string Title => _event.notes + " by " + _event.authorName;
        public string SubjectDesc => _event.subjectDesc;

        public AgendaEventAdapter(AgendaEvent ev)
        {
            _event = ev;
        }
    }

}
