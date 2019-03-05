using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace L4D2ModManager
{
    public class StringDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string)
            {
                var reflections = (parameter as string).Split('.');
                object v = value;
                foreach(var reflection in reflections)
                {
                    v = v.GetType().InvokeMember(reflection, System.Reflection.BindingFlags.GetProperty, null, v, null);
                }
                return v.Display();
            }
            return value.Display();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
