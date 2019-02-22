using System.Linq;
using System.Windows;

namespace L4D2ModManager
{
    static class StringAdapter
    {
        static private bool IsInitialized = false;
        static private string GetFromXAML(string key)
        {
            if (!IsInitialized)
            {
                LanguageHelper.LoadLanguageFile(Configure.Language);
                IsInitialized = true;
            }
            if(Application.Current.Resources.MergedDictionaries[0].Contains(key))
                return (string)Application.Current.Resources.MergedDictionaries[0][key];
            return null;
        }

        static public string GetResource(string key)
        {
            string ret = GetFromXAML(key);
            if (ret == null)
                return key.Split('_').Aggregate((a, b) => a + ' ' + b);
            return ret;
        }

        static public string GetInfo(string key)
        {
            string info = GetFromXAML("Info_" + key);
            Logging.Assert(info != null);
            return info;
        }

        static public string GetString<T>(this T o)
        {
            string key = null;
            if (o.GetType().IsEnum)
                key = "Enum_" + o.GetType().Name + '_' + o.ToString();

            if (key != null)
            {
                string value = GetFromXAML(key);
                if (value != null)
                    return value;
            }

            return o.ToString();
        }
    }
}
