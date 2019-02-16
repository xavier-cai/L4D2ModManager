using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace L4D2ModManager
{
    public class ListviewCollisionColorConverter : IValueConverter
    {
        private SolidColorBrush NormalBrush = new SolidColorBrush(Colors.LightGreen);
        private SolidColorBrush CollisionBrush = new SolidColorBrush(Colors.Red);
        private SolidColorBrush IgnoreBrush = new SolidColorBrush(Colors.Yellow);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as L4D2MM.ModInfo;
            Brush brush = NormalBrush;
            if (item.CanSetOff)
            {
                if (item.IsHaveCollision)
                    brush = CollisionBrush;
                if (item.IsIgnoreCollision)
                    brush = IgnoreBrush;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
