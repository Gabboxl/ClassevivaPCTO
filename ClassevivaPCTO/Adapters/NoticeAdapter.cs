using ClassevivaPCTO.Utils;

namespace ClassevivaPCTO.Adapters
{
    public class NoticeAdapter
    {
        public Notice CurrentObject;

        public string FromToValidDate
        {
            get
            {
                return "valida dal " + CurrentObject.cntValidFrom.ToString("dd/MM/yyyy") + " al " + CurrentObject.cntValidTo.ToString("dd/MM/yyyy");
            }
            
        }

        public string Category
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentObject.cntCategory))
                {
                    return "";
                }
                else
                {
                    return CurrentObject.cntCategory;
                }
            }
        }


        public NoticeAdapter(Notice ev)
        {
            CurrentObject = ev;
        }
    }
}
