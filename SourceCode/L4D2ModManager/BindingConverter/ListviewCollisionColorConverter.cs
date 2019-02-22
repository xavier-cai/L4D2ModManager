using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace L4D2ModManager
{
    public class ListviewCollisionColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as L4D2MM.ModInfo;
            Color color = Configure.View.IndicatorNormal;
            if (item.CanSetOff)
            {
                if (item.IsHaveCollision)
                    color = Configure.View.IndicatorCollision;
                if (item.IsIgnoreCollision)
                    color = Configure.View.IndicatorIgnore;
            }
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
