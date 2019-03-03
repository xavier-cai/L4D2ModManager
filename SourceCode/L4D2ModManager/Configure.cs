using System.Windows.Media;

namespace L4D2ModManager
{
    class Configure
    {
        static public bool ReleaseLogToFile
        {
            get { return ContentInstance.ReleaseLogToFile; }
            set { ContentInstance.ReleaseLogToFile = value; }
        }
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

        static public ViewContent View
        {
            get { return ContentInstance.View; }
        }

        public class ViewContent
        {
            public double FontSize = 12.0;
            public System.Windows.Size WindowSize = new System.Windows.Size(860, 580);
            public Color IndicatorNormal = Colors.LightGreen;
            public Color IndicatorCollision = Colors.Red;
            public Color IndicatorIgnore = Colors.Yellow;
            public Color StateUnregisted = Colors.ForestGreen;
            public Color StateUnsubscribed = Colors.Black;
            public Color StateMiss = Colors.OrangeRed;
            public Color StateOff = Colors.OrangeRed;
            public Color StateOn = Colors.MediumSpringGreen;
        }

        class Content
        {
            public bool ReleaseLogToFile { get; set; }
            public string InstallPath { get; set; }
            public bool EnableSteam { get; set; }
            public bool EnableReadVpk { get; set; }
            public bool EnableAddons { get; set; }
            public string Language { get; set; }
            public bool DelegateSteam { get; set; }
            public ViewContent View { get; } = new ViewContent();

            public Content()
            {
                ReleaseLogToFile = true;
                InstallPath = "";
                EnableSteam = true;
                EnableReadVpk = true;
                EnableAddons = true;
                DelegateSteam = true;
                Language = null;
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
                if(_ContentInstance.Language == null)
                {
                    var language = new WindowLanguage();
                    language.Init = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                    language.FontSize = 14.0;
                    language.ShowDialog();
                    if (language.Result == null)
                        System.Windows.Application.Current.Shutdown();
                    _ContentInstance.Language = language.Result as string;
                }
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
