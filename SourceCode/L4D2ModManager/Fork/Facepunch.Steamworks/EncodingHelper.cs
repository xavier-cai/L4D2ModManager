using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facepunch.Steamworks
{
    static class EncodingHelper
    {
        public static string Decode(this byte[] bytes, Encoding encoding)
        {
            int length = encoding.GetDecoder().GetCharCount(bytes, 0, bytes.Length);
            char[] decode = new char[length];
            int did = encoding.GetDecoder().GetChars(bytes, 0, bytes.Length, decode, 0);
            return string.Concat(decode.Reverse().SkipWhile(c => c == '\0').Reverse());
        }

        public static string Decode(this byte[] bytes)
        {
            return bytes.Decode(Encoding.UTF8);
        }
    }
}
