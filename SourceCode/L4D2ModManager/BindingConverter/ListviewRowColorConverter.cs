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
    public class ListviewRowColorConverter : IValueConverter
    {
        private SolidColorBrush NormalBrush = new SolidColorBrush(Colors.White);
        private SolidColorBrush CollisionBrush = new SolidColorBrush(Color.FromArgb(0x20, 0xff, 0x00, 0x00));
        private SolidColorBrush IgnoreBrush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0xff, 0x00));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as System.Windows.Controls.ListViewItem;
            var context = item.DataContext as ViewItem;
            Brush brush = NormalBrush;
            if (context.Mod.CanSetOff)
            {
                if (context.Mod.IsHaveCollision)
                    brush = CollisionBrush;
                if (context.Mod.IsIgnoreCollision)
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
