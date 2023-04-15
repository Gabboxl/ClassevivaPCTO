namespace ClassevivaPCTO.Utils
{
    public class AgendaEventAdapter : BindableBase.BindableBase
    {
        public AgendaEvent CurrentObject;

        public string Title { 
            get {

                if(string.IsNullOrEmpty(CurrentObject.subjectDesc))
                {
                    return CurrentObject.authorName;
                } else
                {
                    return CurrentObject.subjectDesc + " (" + CurrentObject.authorName + ")";
                }

            
            }
        }

        public string Notes => CurrentObject.notes;

        public AgendaEventAdapter(AgendaEvent ev)
        {
            CurrentObject = ev;
        }
    }

}
