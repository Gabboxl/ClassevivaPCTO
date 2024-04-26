using System;
using System.Collections;
using Windows.UI.Xaml.Data;

namespace ClassevivaPCTO.Converters
{
    public class EmptyListToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //parametro per invertire il risultato
            bool param = true;

            if (parameter != null)
                param = System.Convert.ToBoolean(parameter);

            IList list = (IList) value;

            return (list != null && list.Count != 0) == param;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
}