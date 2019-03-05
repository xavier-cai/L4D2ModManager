using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace L4D2ModManager
{
    public static class CustomInformation
    {
        public class ViewList
        {
            public string Header { get; set; }
            public bool TryTranslate { get; set; } = true;
            public string Reflection { get; set; }
            public double Width { get; set; } = 50;
            public System.Windows.TextAlignment TextAlignment { get; set; } = System.Windows.TextAlignment.Center;
            internal ViewList() { }
            internal ViewList(string header, bool tryTrans, string reflection, double width)
            {
                Header = header;
                TryTranslate = tryTrans;
                Reflection = reflection;
                Width = width;
            }
            internal ViewList(string name, double width)
            {
                Header = name;
                TryTranslate = true;
                Reflection = name;
                Width = width;
            }
        }

        public class ViewBox
        {
            public string Header { get; set; }
            public bool TryTranslate { get; set; }
            public string Reflection { get; set; }
            public Color Color { get; set; } = Colors.Gray;
            internal ViewBox() { }
            internal ViewBox(string header, bool tryTrans, string reflection, Color color)
            {
                Header = header;
                Reflection = reflection;
                TryTranslate = tryTrans;
                Color = color;
            }
            internal ViewBox(string name, Color color)
            {
                Header = name;
                TryTranslate = true;
                Reflection = name;
                Color = color;
            }
        }

        public class InstanceClass
        {
            public List<ViewList> ViewLists = new List<ViewList>();
            public List<ViewBox> ViewBoxes = new List<ViewBox>();
            internal static InstanceClass GetInstanceClass()
            {
                var instance = new InstanceClass();
                if (!InitFromFile(ref instance.ViewLists, ViewListFile))
                {
                    //instance.ViewLists.Add(new ViewList("Version", 100));
                }
                if (!InitFromFile(ref instance.ViewBoxes, ViewBoxFile))
                {
                    instance.ViewBoxes.Add(new ViewBox("Author", Colors.Gray));
                    instance.ViewBoxes.Add(new ViewBox("Tags", Colors.Gray));
                    instance.ViewBoxes.Add(new ViewBox("Description", Colors.Black));
                }
                return instance;
            }
            private const string ViewListFile = "view-list.ini";
            private const string ViewBoxFile = "view-box.ini";
            private static bool InitFromFile<T>(ref T o, string file) where T : class
            {
                if (!System.IO.File.Exists(file))
                    return false;
                try
                {
                    o = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(System.IO.File.ReadAllText(file));
                    return true;
                }
                catch (Exception e)
                {
                    Logging.Log(e.Message + "\r\n" + e.StackTrace, "LoadFile");
                }
                return false;
            }
            public void Save()
            {
                System.IO.File.WriteAllText(ViewListFile, Newtonsoft.Json.JsonConvert.SerializeObject(ViewLists));
                System.IO.File.WriteAllText(ViewBoxFile, Newtonsoft.Json.JsonConvert.SerializeObject(ViewBoxes));
            }
        }


        
        private static InstanceClass _instance = null;
        public static InstanceClass Instance
        {
            get
            {
                if (_instance == null)
                    _instance = InstanceClass.GetInstanceClass();
                return _instance;
            }
        }
    }
}
