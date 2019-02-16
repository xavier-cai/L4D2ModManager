using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace L4D2ModManager
{
    public class LanguageHelper
    {
        private static string Current = null;
        public static void LoadLanguageFile(string language)
        {
            if (Current == null || !language.Equals(Current))
            {
                string file = "/Resources/Langs/" + language + ".xaml";
                var resource = new ResourceDictionary()
                {
                    Source = new Uri(file, UriKind.RelativeOrAbsolute)
                };
                if (Application.Current.Resources.MergedDictionaries.Count < 1)
                    Application.Current.Resources.MergedDictionaries.Add(resource);
                else
                    Application.Current.Resources.MergedDictionaries[0] = resource;
                Current = language;
            }
        }
    }
}
