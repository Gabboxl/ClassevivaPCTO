using System;
using System.Collections.Generic;
using System.Linq;
using ClassevivaPCTO.Utils;
using Windows.UI.Xaml.Media;

namespace ClassevivaPCTO.Adapters
{
    public class SubjectAdapter
    {
        public readonly List<Grade> SubjectGrades;
        public readonly Subject Subject;

        public string Teachers
        {
            get
            {
                return String.Join(", ", Subject.teachers.Select(t => t.teacherName));
            }
        }

        public float? Average
        {
            get
            {
                return VariousUtils.CalcolaMedia(SubjectGrades);
            }
        }

        public int Progress
        {
            get
            {
                return (int) (Average * 10 ?? 0);
            }
        }


        public SubjectAdapter(Subject subject, List<Grade> subjectSubjectGrades)
        {
            SubjectGrades = subjectSubjectGrades;
            Subject = subject;
        }
    }
}