using System;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ClassevivaPCTO.Converters
{
    public class EmptyListToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //parametro per invertire il risultato
            bool param = true;

            if (parameter != null)
                param = System.Convert.ToBoolean(parameter);

            IList list = (IList) value;

            return (list != null && list.Count != 0) == param ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
}