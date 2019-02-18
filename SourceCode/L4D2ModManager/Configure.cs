using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L4D2ModManager
{
    class Configure
    {
        static public string InstallPath
        {
            get { return ContentInstance.InstallPath; }
            set { ContentInstance.InstallPath = value; }
        }
        static public bool EnableSteam
        {
            get { return ContentInstance.EnableSteam; }
            set { ContentInstance.EnableSteam = value; }
        }
        static public bool EnableReadVpk
        {
            get { return ContentInstance.EnableReadVpk; }
            set { ContentInstance.EnableReadVpk = value; }
        }
        static public bool EnableAddons
        {
            get { return ContentInstance.EnableAddons; }
            set { ContentInstance.EnableAddons = value; }
        }
        static public string Language
        {
            get { return ContentInstance.Language; }
            set { ContentInstance.Language = value; }
        }

        static public bool DelegateSteam
        {
            get { return ContentInstance.DelegateSteam; }
            set { ContentInstance.DelegateSteam = value; }
        }

        class Content
        {
            public string InstallPath { get; set; }
            public bool EnableSteam { get; set; }
            public bool EnableReadVpk { get; set; }
            public bool EnableAddons { get; set; }
            public string Language { get; set; }
            public bool DelegateSteam { get; set; }
            public Content()
            {
                InstallPath = "";
                EnableSteam = true;
                EnableReadVpk = false;
                EnableAddons = true;
                Language = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                DelegateSteam = true;
            }
        }
        
        private const string ConfigurePath = "config.ini";

        static private Content _ContentInstance = null;
        static private Content ContentInstance
        {
            get
            {
                if (_ContentInstance == null)
                    LoadConfigure();
                if (_ContentInstance == null)
                    _ContentInstance = new Content();
                return _ContentInstance;
            }
        }

        static private Content ReadConfigure(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var read = System.IO.File.ReadAllText(path);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Content>(read);
            }
            return null;
        }

        static public void LoadConfigure(string path = ConfigurePath)
        {
            _ContentInstance = ReadConfigure(path);
        }

        static public void SaveConfigure(string path = ConfigurePath)
        {
            System.IO.File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(ContentInstance));
        }
    }
}
