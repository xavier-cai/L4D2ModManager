using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace L4D2ModManager
{
    static class CtlExtension
    {
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
    }
}
