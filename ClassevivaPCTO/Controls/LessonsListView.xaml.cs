using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.DataModels;
using ClassevivaPCTO.Utils;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Controls
{
    public sealed partial class LessonsListView : UserControl
    {

        public bool IsSingleList
        {
            get { return (bool)GetValue(IsSingleListProperty); }
            set
            {
                SetValue(IsSingleListProperty, value);
            }
        }

        private static readonly DependencyProperty IsSingleListProperty =
            DependencyProperty.Register(
                nameof(IsSingleList),
                typeof(bool),
                typeof(LessonsListView),
                new PropertyMetadata(false, null));


        public List<Lesson> ItemsSource
        {
            get { return (List<Lesson>)GetValue(ItemsSourceProperty); }
            set {
                SetValue(ItemsSourceProperty, value); 
            }
        }

        private static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(List<Lesson>),
                typeof(LessonsListView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentInstance = (LessonsListView)d;

            var newValue = e.NewValue as List<Lesson>;

            List<Lesson> orderedlessons = null;

            if (currentInstance.IsSingleList)
            {

                //order lessons by first by date and then by evtHPos desc (so that the first lesson of the day is on top)
                orderedlessons = newValue?.OrderByDescending(x => x.evtDate).ThenByDescending(x => x.evtHPos).ToList();

            }
            else
            {
                orderedlessons = newValue?.OrderBy(x => x.evtHPos).ToList();
            }


            //remove duplicates based on lessonArg and authorname and same day and increment evtDuration if it is a duplicate
            foreach (var lesson in orderedlessons.ToList())
            {
                var duplicates = orderedlessons
                    .Where(
                        x =>
                            x.lessonArg == lesson.lessonArg
                            && x.authorName == lesson.authorName
                            && x.evtDate == lesson.evtDate
                    )
                    .ToList();
                if (duplicates.Count > 1)
                {
                    lesson.evtDuration += duplicates[1].evtDuration;
                    orderedlessons.Remove(duplicates[1]);
                }
            }



            //orderedlessons = orderedlessons.GroupBy(x => x.lessonArg).Select(x => x.First()).ToList();


            var eventAdapters = orderedlessons.Select(evt => new LessonAdapter(evt)).ToList();

            currentInstance.listView.ItemsSource = eventAdapters;
        }


        public LessonsListView()
        {
            this.InitializeComponent();
        }


    }
}
