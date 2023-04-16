using ClassevivaPCTO.Utils;

namespace ClassevivaPCTO.Adapters
{
    public class LessonAdapter : BindableBase.BindableBase
    {
        public Lesson CurrentObject;

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

        public string EventType
        {
            get
            {

                return "[" + CurrentObject.evtCode + "]";


            }
        }


        public LessonAdapter(Lesson ev)
        {
            CurrentObject = ev;
        }
    }

}
