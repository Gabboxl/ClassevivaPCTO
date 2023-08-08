using ClassevivaPCTO.Utils;
using Windows.UI.Xaml.Media;

namespace ClassevivaPCTO.Adapters
{
    public class LessonAdapter
    {
        public readonly Lesson CurrentObject;

        public SolidColorBrush ColorBrush
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();
                ColorGenerator generator = new ColorGenerator(0.5, 0.8, 0.6);

                System.Drawing.Color colorTest = generator.GetColor((int)CurrentObject.subjectId); // generates a unique color for number

                Windows.UI.Color uwpColor = Windows.UI.Color.FromArgb(colorTest.A, colorTest.B, colorTest.G, colorTest.R);

                brush.Color = uwpColor;

                return brush;
            }
        }

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
                    return "1 ora";
                }
                else
                {
                    return CurrentObject.evtDuration + " ore";
                }
            }
        }

        public LessonAdapter(Lesson ev)
        {
            CurrentObject = ev;
        }
    }
}
