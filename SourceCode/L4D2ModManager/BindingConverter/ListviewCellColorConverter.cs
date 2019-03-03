using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace L4D2ModManager
{
    public class ListviewCellColorConverter : IValueConverter
    {
        class ColorMatcher
        {
            private Dictionary<L4D2MM.ModState, Func<Color>> m_matches;

            public ColorMatcher()
            {
                m_matches = new Dictionary<L4D2MM.ModState, Func<Color>>();
                m_matches.Add(L4D2MM.ModState.Unregisted, () => Configure.View.StateUnregisted);
                m_matches.Add(L4D2MM.ModState.Unsubscribed, () => Configure.View.StateUnsubscribed);
                m_matches.Add(L4D2MM.ModState.Miss, () => Configure.View.StateMiss);
                m_matches.Add(L4D2MM.ModState.Off, () => Configure.View.StateOff);
                m_matches.Add(L4D2MM.ModState.On, () => Configure.View.StateOn);
            }

            public Color Match(L4D2MM.ModState key)
            {
                if (m_matches.ContainsKey(key))
                    return m_matches[key]();
                throw new ArgumentException();
            }
        }

        static ColorMatcher Matcher = new ColorMatcher();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach(var v in Enum.GetValues(typeof(L4D2MM.ModState)))
                if(v.GetString().Equals(value as string))
                    return new SolidColorBrush(Matcher.Match((L4D2MM.ModState)v));
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
