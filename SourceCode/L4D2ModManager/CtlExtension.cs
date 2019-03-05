using System.Text;
using System.Windows;
using System.Linq;

namespace L4D2ModManager
{
    static class CtlExtension
    {
        public static bool IsNumber(this string str)
        {
            if (str == null || str.Length <= 0)
                return false;
            foreach (var c in str)
                if (c < '0' || c > '9')
                    return false;
            return true;
        }

        public static void AppendAndScroll(this System.Windows.Controls.TextBox ctl, string text)
        {
            ctl.AppendText(text);
            ctl.ScrollToEnd();
        }

        public static void SetSource(this System.Windows.Controls.Image ctl, System.IO.MemoryStream ms)
        {
            if (ms != null)
                ctl.Source = (System.Windows.Media.ImageSource)new System.Windows.Media.ImageSourceConverter().ConvertFrom(ms);
            else
                ctl.Source = null;
        }

        public static void SetSize(this System.Windows.FrameworkElement ctl, Size size)
        {
            ctl.Width = size.Width;
            ctl.Height = size.Height;
        }

        public static void SetSize(this System.Windows.FrameworkElement ctl, double width, double height)
        {
            ctl.Width = width;
            ctl.Height = height;
        }

        public static bool ConfirmBox(string text, string header = "")
        {
            System.Windows.MessageBoxResult ret = System.Windows.MessageBox.Show(text, header, System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Question);
            return ret == System.Windows.MessageBoxResult.OK;
        }

        public static string ToFileSize(this long bytes)
        {
            long size = bytes;
            long left = 0;
            int count = 0;
            while(size > 1024 && count < 3)
            {
                left = size % 1024;
                size /= 1024;
                count++;
            }
            string ret = size.ToString() + '.' + (left * 100 / 1024).ToString();
            switch(count)
            {
                case 0:ret += "B"; break;
                case 1:ret += "KB";break;
                case 2:ret += "MB";break;
                case 3:ret += "GB";break;
            }
            return ret;
        }

        public static string DecodeTo(this string o, Encoding from, Encoding to)
        {
            byte[] bytes = from.GetBytes(o);
            int length = to.GetDecoder().GetCharCount(bytes, 0, bytes.Length);
            char[] decode = new char[length];
            int did = to.GetDecoder().GetChars(bytes, 0, bytes.Length, decode, 0);
            return string.Concat(decode);
        }

        public static string Display<T>(this System.Collections.Generic.IEnumerable<T> o)
        {
            if (o.Count() <= 0)
                return "";
            return o.Aggregate(" ", (a, b)=>a+", "+b).Substring(2);
        }

        public static string Display(this object o)
        {
            if (o is null)
                return "";
            if (o is string)
                return o as string;
            if (o is System.Collections.IEnumerable)
            {
                var array = (o as System.Collections.IEnumerable).Cast<object>();
                if (array.Count() <= 0)
                    return "";
                return array.Aggregate(" ", (a, b) => a + ", " + b).Substring(2);
            }
            return o.ToString();
        }
    }
}
