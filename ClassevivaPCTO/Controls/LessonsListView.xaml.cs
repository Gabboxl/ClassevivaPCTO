using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Controls
{
    public sealed partial class LessonsListView : UserControl
    {

        public List<Lesson> ItemsSource
        {
            get { return (List<Lesson>)GetValue(ItemsSourceProperty); }
            set {
                SetValue(ItemsSourceProperty, value); 
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(List<Lesson>),
                typeof(GradesListView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentInstance = (LessonsListView)d;

            var newValue = e.NewValue as List<Lesson>;


            //order lessons by evtHPos
            var orderedlessons = newValue?.OrderBy(x => x.evtHPos).ToList();

            //remove duplicates based on lessonArg and authorname and increment evtDuration it it is a duplicate
            foreach (var lesson in orderedlessons.ToList())
            {
                var duplicates = orderedlessons
                    .Where(
                        x =>
                            x.lessonArg == lesson.lessonArg
                            && x.authorName == lesson.authorName
                    )
                    .ToList();
                if (duplicates.Count > 1)
                {
                    lesson.evtDuration += duplicates[1].evtDuration;
                    orderedlessons.Remove(duplicates[1]);
                }
            }

            //orderedlessons = orderedlessons.GroupBy(x => x.lessonArg).Select(x => x.First()).ToList();


            var eventAdapters = orderedlessons?.Select(evt => new LessonAdapter(evt)).ToList();

            currentInstance.listView.ItemsSource = eventAdapters;
        }


        public LessonsListView()
        {
            this.InitializeComponent();
        }


    }
}
