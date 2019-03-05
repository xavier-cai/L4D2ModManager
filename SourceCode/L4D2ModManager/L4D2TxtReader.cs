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

        private static char[] LineEnd = new char[] { '\r', '\n' };
        public void LoadAddonInfo(Stream stream)
        {
            Values.Clear();
            m_encoding = DetectEncoding(stream);

            string[] contents;
            {
                char last;
                ReadUntill(stream, c => c == '{', out last);
                if (last != '{')
                    return;
                var content = ReadUntill(stream, c => c == '}', out last);
                if (last != '}')
                    return;
                contents = content.Split('\n');
            }
            for (int i = 0; i < contents.Length - 1; i++)
                contents[i] += '\n';

            Func<string, string> RemoveSpaceAtFront = s =>
            {
                char c = s.FirstOrDefault(ch => ch != ' ' && ch != '\t');
                if (c == default(char))
                    return "";
                return s.Substring(s.IndexOf(c));
            };

            Func<string, string> PeekWord = s =>
            {
                string ret = "";
                foreach (var c in s)
                    if (LineEnd.Contains(c) || c == ' ' || c == '\t') break;
                    else ret += c;
                return ret;
            };

            Func<string, int, string> PeekValue = (str, cnt) =>
            {
                var s = RemoveSpaceAtFront(str);
                var first = PeekWord(s);
                if (first.Length <= 0)
                    return "";
                if (first[0] != '"')
                    return first;
                if (cnt == 0)
                    return first;
                if (cnt == 1)
                    return "";
                var sub = s.Substring(1, s.LastIndexOf('"') - 1);
                if(cnt % 2 == 1)
                {
                    sub = sub.Substring(0, sub.LastIndexOf('"'));
                }
                return sub;
            };

            string key = "";
            string value = "";
            int count = 0;

            foreach (var v in contents)
            {
                var line = v;
                if (count % 2 == 0)
                {
                    var first = PeekWord(RemoveSpaceAtFront(line));
                    if (first.ToLower().Contains("addon"))
                    {
                        if (key.Length > 0)
                        {
                            AddValue(key, PeekValue(value, count));
                        }
                        key = first;
                        value = RemoveSpaceAtFront(RemoveSpaceAtFront(line).Substring(first.Length + 1));
                        count = value.Count(c => c.Equals('"')); ;
                    }
                    else
                    {
                        value += v;
                        count += v.Count(c => c.Equals('"'));
                    }
                }
                else
                {
                    value += v;
                    count += v.Count(c => c.Equals('"'));
                }
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
