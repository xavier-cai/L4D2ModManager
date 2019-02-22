using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace L4D2ModManager
{
    public class ListviewTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as L4D2MM.ModInfo;
            StringBuilder sb = new StringBuilder();
            if(item.IsHaveCollision)
            {
                sb.Append(StringAdapter.GetInfo("HaveCollision") + " : ");
                foreach(var v in item.Resources)
                {
                    if (v.Value)
                        sb.Append("\r\n-" + v.Key.Display);
                }
            }
            else
            {
                if (item.IsIgnoreCollision)
                    sb.Append(StringAdapter.GetInfo("IgnoredCollision"));
                else
                    sb.Append(StringAdapter.GetInfo("NoCollision"));
            }
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
