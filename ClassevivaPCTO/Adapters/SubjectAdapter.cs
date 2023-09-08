﻿using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using ClassevivaPCTO.Utils;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using ClassevivaPCTO.Helpers.Palettes;
using ClassevivaPCTO.Services;

namespace ClassevivaPCTO.Adapters
{
    public class SubjectAdapter
    {
        public readonly List<Grade> SubjectGrades;
        public readonly Subject Subject;

        public string Teachers
        {
            get { return String.Join(", ", Subject.teachers.Select(t => t.teacherName)); }
        }

        public float Average
        {
            get { return VariousUtils.CalcolaMedia(SubjectGrades); }
        }

        public string AverageString
        {
            get { return Average.ToString("0.0"); }
        }

        public float Progress
        {
            get { return (Average * 10); }
        }

        public FontIcon AverageVariationIcon
        {
            get
            {
                //get list of grades without the grade that has the most recent date
                List<Grade> gradesWithoutLastGrade =
                    SubjectGrades.Where(g => g.evtDate != SubjectGrades.Max(g => g.evtDate)).ToList();

                float averageWithoutLastGrade = VariousUtils.CalcolaMedia(gradesWithoutLastGrade);

                IPalette currentPalette = PaletteSelectorService.PaletteClass;

                SolidColorBrush brush = new SolidColorBrush();

                string iconString;

                if (averageWithoutLastGrade > Average)
                {
                    iconString = "\uF0AE";
                    brush.Color = currentPalette.ColorRed;
                }
                else if (averageWithoutLastGrade < Average)
                {
                    iconString = "\uF0AD";
                    brush.Color = currentPalette.ColorGreen;
                }
                else
                {
                    iconString = "\uE94E";
                    brush.Color = currentPalette.ColorOrange;
                }

                FontIcon icon = new FontIcon();
                icon.Glyph = iconString;
                icon.Foreground = brush;

                return icon;
            }
        }


        public SubjectAdapter(Subject subject, List<Grade> subjectSubjectGrades)
        {
            SubjectGrades = subjectSubjectGrades;
            Subject = subject;
        }
    }
}