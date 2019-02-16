using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection
{
    public sealed class Constants
    {
        public static string CHARSET_ISO_2022_JP = "ISO-2022-JP";
        public static string CHARSET_ISO_2022_CN = "ISO-2022-CN";
        public static string CHARSET_ISO_2022_KR = "ISO-2022-KR";
        public static string CHARSET_ISO_8859_5 = "ISO-8859-5";
        public static string CHARSET_ISO_8859_7 = "ISO-8859-7";
        public static string CHARSET_ISO_8859_8 = "ISO-8859-8";
        public static string CHARSET_BIG5 = "BIG5";
        public static string CHARSET_GB18030 = "GB18030";
        public static string CHARSET_EUC_JP = "EUC-JP";
        public static string CHARSET_EUC_KR = "EUC-KR";
        public static string CHARSET_EUC_TW = "EUC-TW";
        public static string CHARSET_SHIFT_JIS = "SHIFT_JIS";
        public static string CHARSET_IBM855 = "IBM855";
        public static string CHARSET_IBM866 = "IBM866";
        public static string CHARSET_KOI8_R = "KOI8-R";
        public static string CHARSET_MACCYRILLIC = "MACCYRILLIC";
        public static string CHARSET_WINDOWS_1251 = "WINDOWS-1251";
        public static string CHARSET_WINDOWS_1252 = "WINDOWS-1252";
        public static string CHARSET_WINDOWS_1253 = "WINDOWS-1253";
        public static string CHARSET_WINDOWS_1255 = "WINDOWS-1255";
        public static string CHARSET_UTF_8 = "UTF-8";
        public static string CHARSET_UTF_16BE = "UTF-16BE";
        public static string CHARSET_UTF_16LE = "UTF-16LE";
        public static string CHARSET_UTF_32BE = "UTF-32BE";
        public static string CHARSET_UTF_32LE = "UTF-32LE";

        // WARNING: Listed below are charsets which Java does not support.
        public static string CHARSET_HZ_GB_2312 = "HZ-GB-2312"; // Simplified Chinese
        public static string CHARSET_X_ISO_10646_UCS_4_3412 = "X-ISO-10646-UCS-4-3412"; // Malformed UTF-32
        public static string CHARSET_X_ISO_10646_UCS_4_2143 = "X-ISO-10646-UCS-4-2143"; // Malformed UTF-32
    }
}
