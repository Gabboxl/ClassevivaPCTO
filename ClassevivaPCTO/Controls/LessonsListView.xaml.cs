using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CloneExtensions;

namespace ClassevivaPCTO.Controls
{
    public sealed partial class LessonsListView : UserControl, INotifyPropertyChanged
    {

        public bool EnableEmptyAlert
        {
            get { return (bool) GetValue(EnableEmptyAlertProperty); }
            set { SetValue(EnableEmptyAlertProperty, value); }
        }

        private static readonly DependencyProperty EnableEmptyAlertProperty =
            DependencyProperty.Register(
                nameof(EnableEmptyAlert),
                typeof(bool),
                typeof(LessonsListView),
                new PropertyMetadata(false, null));

        private bool _showEmptyAlert;

        public bool ShowEmptyAlert
        {
            get { return _showEmptyAlert; }
            set
            {
                SetField(ref _showEmptyAlert, value);
            }
        }

        public bool IsSingleSubjectList
        {
            get { return (bool) GetValue(IsSingleSubjectListProperty); }
            set { SetValue(IsSingleSubjectListProperty, value); }
        }

        private static readonly DependencyProperty IsSingleSubjectListProperty =
            DependencyProperty.Register(
                nameof(IsSingleSubjectList),
                typeof(bool),
                typeof(LessonsListView),
                new PropertyMetadata(false, null));


        public List<Lesson> ItemsSource
        {
            get { return (List<Lesson>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(List<Lesson>),
                typeof(LessonsListView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentInstance = (LessonsListView) d;

            var newValue = e.NewValue as List<Lesson>;

            List<Lesson>? orderedlessons;

            if (currentInstance.IsSingleSubjectList)
            {
                //order lessons by first by date and then by evtHPos desc (so that the first lesson of the day is on top)
                orderedlessons = newValue?.OrderByDescending(x => x.evtDate).ThenByDescending(x => x.evtHPos).ToList();


                //set the listview dattemplate to 
                currentInstance.listView.ItemTemplate =
                    currentInstance.Resources["LessonListViewExpressiveDataTemplate"] as DataTemplate;
            }
            else
            {
                orderedlessons = newValue?.OrderBy(x => x.evtHPos).ToList();

                currentInstance.listView.ItemTemplate =
                    currentInstance.Resources["LessonListViewDataTemplate"] as DataTemplate;
            }

            //copy the list so that we can remove duplicates without affecting the original list
            var copiedOrderedLessons = orderedlessons.GetClone();


            //remove duplicates based on lessonArg and authorname and same day and increment evtDuration if it is a duplicate
            foreach (var lesson in copiedOrderedLessons.ToList())
            {
                var duplicates = copiedOrderedLessons
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
                    copiedOrderedLessons.Remove(duplicates[1]);
                }
            }


            //orderedlessons = orderedlessons.GroupBy(x => x.lessonArg).Select(x => x.First()).ToList();


            var eventAdapters = copiedOrderedLessons.Select(evt => new LessonAdapter(evt)).ToList();

            currentInstance.listView.ItemsSource = eventAdapters;


            currentInstance.ShowEmptyAlert = (newValue == null || newValue.Count == 0) && currentInstance.EnableEmptyAlert;
        }


        public LessonsListView()
        {
            this.InitializeComponent();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}