using ClassevivaPCTO.Utils;

namespace ClassevivaPCTO.Adapters
{
    public class LessonAdapter : BindableBase.BindableBase
    {
        public Lesson CurrentObject;

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

        public string EventType
        {
            get
            {

                return "[" + CurrentObject.evtCode + "]";
            }
        }

        public string Durata
        {
            get
            {
                if (CurrentObject.evtDuration == 1)
                {
                    return "1h";
                }
                else
                {
                    return CurrentObject.evtDuration + "hh";
                }
            }
        }

        public LessonAdapter(Lesson ev)
        {
            CurrentObject = ev;
        }
    }
}
