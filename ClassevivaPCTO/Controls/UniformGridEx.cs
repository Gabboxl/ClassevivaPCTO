using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace ClassevivaPCTO.Controls
{
    public class UniformGridEx : UniformGrid
    {
        public static readonly DependencyProperty MinRowHeightProperty = DependencyProperty.Register(
            nameof(MinRowHeight), typeof(double), typeof(UniformGrid), new PropertyMetadata(0d));

        private int _columns;
        private int _rows;

        public double MinRowHeight
        {
            get { return (double) GetValue(MinRowHeightProperty); }
            set { SetValue(MinRowHeightProperty, value); }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            UpdateComputedValues();
            var calculatedSize = base.MeasureOverride(constraint);
            if (MinRowHeight > 0)
            {
                calculatedSize = new Size(calculatedSize.Width, Math.Max(calculatedSize.Height, _rows * MinRowHeight));
            }

            return calculatedSize;
        }

        private void UpdateComputedValues()
        {
            _columns = Columns;
            _rows = Rows;

            if (FirstColumn >= _columns)
            {
                FirstColumn = 0;
            }

            if ((_rows == 0) || (_columns == 0))
            {
                int nonCollapsedCount = 0;

                for (int i = 0, count = Children.Count; i < count; ++i)
                {
                    UIElement child = Children[i];
                    if (child.Visibility != Visibility.Collapsed)
                    {
                        nonCollapsedCount++;
                    }
                }

                if (nonCollapsedCount == 0)
                {
                    nonCollapsedCount = 1;
                }

                if (_rows == 0)
                {
                    if (_columns > 0)
                    {
                        _rows = (nonCollapsedCount + FirstColumn + (_columns - 1)) / _columns;
                    }
                    else
                    {
                        _rows = (int) Math.Sqrt(nonCollapsedCount);
                        if ((_rows * _rows) < nonCollapsedCount)
                        {
                            _rows++;
                        }

                        _columns = _rows;
                    }
                }
                else if (_columns == 0)
                {
                    _columns = (nonCollapsedCount + (_rows - 1)) / _rows;
                }
            }
        }
    }
}