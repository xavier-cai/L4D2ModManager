using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace L4D2ModManager
{
    class L4D2TxtReader
    {
        public enum TxtType
        {
            AddonInfo,
            AddonList
        }

        public List<KeyValuePair<string, string>> Values { get; private set; }
        private Encoding m_encoding;

        public L4D2TxtReader(string file, TxtType type)
        {
            using (var fs = new FileStream(file, FileMode.Open))
            {
                Run(fs, type);
            }
        }

        public L4D2TxtReader(Stream stream, TxtType type)
        {
            Run(stream, type);
        }

        private L4D2TxtReader()
        {
            Values = new List<KeyValuePair<string, string>>();
        }

        private void Run(Stream stream, TxtType type)
        {
            Values = new List<KeyValuePair<string, string>>();
            switch (type)
            {
                case TxtType.AddonInfo: LoadAddonInfo(stream); break;
                case TxtType.AddonList: LoadAddonList(stream); break;
            }
        }

        private Encoding DetectEncoding(Stream stream)
        {
            long position = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);
            var detectionLength = 0;
            var detectionBuffer = new Byte[4096];
            var universalDetector = new Mozilla.UniversalCharacterDetection.UniversalDetector(null);

            while ((detectionLength = stream.Read(detectionBuffer, 0, detectionBuffer.Length)) > 0 && !universalDetector.IsDone())
            {
                universalDetector.HandleData(detectionBuffer, 0, detectionBuffer.Length);
            }
            universalDetector.DataEnd();
            stream.Seek(position, SeekOrigin.Begin);
            if (universalDetector.GetDetectedCharset() != null)
            {
               return System.Text.Encoding.GetEncoding(universalDetector.GetDetectedCharset());
            }
            return System.Text.Encoding.GetEncoding("ASCII");
            //return System.Text.Encoding.Default;
        }

        public void LoadAddonInfo(Stream stream)
        {
            Values.Clear();
            m_encoding = DetectEncoding(stream);

            char[] bounds = new char[] { '\r', '\n', '\t', ' ', '}' };
            char last;

            ReadUntill(stream, c => c == '{', out last);
            if (last == '{')
            {
                while (stream.Position < stream.Length)
                {
                    ReadUntill(stream, c => !bounds.Contains(c), out last);
                    string str = ReadUntill(stream, c => bounds.Contains(c), out last, last);

                    if (last == char.MinValue || last == '}' || str.Length == 0 || !IsKey(str))
                        break;

                    ReadValueForKey(stream, str, bounds);
                }
            }
        }

        private bool IsKey(string str)
        {
            return str.ToLower().Contains("addon");
        }

        private void ReadValueForKey(Stream stream, string key, char[] bound)
        {
            char last;
            string blank = ReadUntill(stream, c => !bound.Contains(c), out last);
            if (last == '\"')
            {
                string v = ReadUntill(stream, c => c == '\"', out last);
                while (stream.Position < stream.Length)
                {
                    Logging.Assert(last == '\"', "Syntax error : unbalanced parentheses, the Key is " + key + ", the Last is " + last);
                    string skip = ReadUntill(stream, c => !bound.Contains(c), out last);
                    if (skip.Length == 0)
                    {
                        v += '\"';
                        v += ReadUntill(stream, c => c == '\"', out last, last);
                    }
                    else
                    {
                        string next = ReadUntill(stream, c => bound.Contains(c), out last, last);
                        if (last != '\"' && IsKey(next))
                        {
                            AddValue(key, v);
                            ReadValueForKey(stream, next, bound);
                            break;
                        }
                        //else
                        v += skip + next + ReadUntill(stream, c => c == '\"', out last, last);
                    }
                }
            }
            else
            {
                string v = ReadUntill(stream, c => bound.Contains(c), out last, last);
                AddValue(key, v);
            }
        }



        public void LoadAddonList(Stream stream)
        {
            Values.Clear();
            m_encoding = DetectEncoding(stream);

            char[] bounds = new char[] { '\r', '\n', '\t', ' ', '}' };
            char last;

            ReadUntill(stream, c => c == '{', out last);
            if (last == '{')
            {
                while (stream.Position < stream.Length)
                {
                    string key = ReadOneListItem(stream);
                    string value = ReadOneListItem(stream);
                    if (key.Length <= 0 || value.Length <= 0)
                        break;
                    AddValue(key, value);
                }
            }
        }

        private string ReadOneListItem(Stream stream)
        {
            char last;
            ReadUntill(stream, c => c == '\"', out last); //skip
            if(last == '\"')
                return ReadUntill(stream, c => c == '\"', out last);
            return "";
        }




        private void AddValue(string key, string value)
        {
            //int i = value.IndexOf("//");
            //if (i != -1)
            //    value = value.Substring(0, i);
            Values.Add(new KeyValuePair<string, string>(key, value));
            //Logging.Log(key + " : " + value, "DEBUG");
        }

        private string ReadUntill(Stream stream, Predicate<char> cond, out char last, char prefix = char.MinValue)
        {
            MemoryStream ms = new MemoryStream();
            if (prefix != char.MinValue)
                ms.WriteByte(Convert.ToByte(prefix));
            last = char.MinValue;
            while (stream.Position < stream.Length)
            {
                int i = stream.ReadByte();
                if(i == -1)
                    break;
                byte b = Convert.ToByte(i);
                char c = Convert.ToChar(b);
                if (cond.Invoke(c))
                {
                    last = c;
                    break;
                }
                ms.WriteByte(b);
            }
            ms.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(ms, m_encoding);
            return sr.ReadToEnd();
        }
    }
}
