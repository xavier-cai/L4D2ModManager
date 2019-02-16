using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace L4D2ModManager
{
    class L4D2MM : IDisposable
    {
        //for get path
        static private string m_ignoreFile = @"ignore.list";
        static private string m_registryPath = @"SOFTWARE\Classes\Applications\left4dead2.exe\shell\open\command";
        static private string m_registryKey = @"";

        //for mods managing
        static private uint m_appid = 550;
        static private string m_dirCore = @"\left4dead2\";
        static private string m_fileList = m_dirCore + @"addonlist.txt";
        static private string m_dirAddons = m_dirCore + @"addons\";
        static private string m_dirWorkshop = m_dirAddons + @"workshop\";

        //basic value
        private string m_path;
        private Facepunch.Steamworks.Client m_steam = null;

        public enum ModState
        {
            Miss, //in list but file not exsit
            Unregisted, //file exsit but not in list
            Unsubscribed, //user unsubscribed the mod
            On,
            Off
        };

        public class ModInfo : L4D2Resource.ResourceHandler
        {
            public ModInfo(L4D2MM manager, string key, ModState state, ModSource source, L4D2Mod mod)
            {
                Manager = manager;
                Key = key;
                ModState = state;
                Source = source;
                Mod = mod;
                IsHaveCollision = false;

                base.CollisionCountChangeHandler = (count) => { IsHaveCollision = count > 0; };

                if (mod != null)
                {
                    foreach (var a in mod.Category)
                        foreach (var b in mod.SubCategory(a))
                            base.AddResource(L4D2Resource.GetResource(a, b));
                }

                if (CanSetOff && !IsIgnoreCollision)
                    base.Regist();
            }
            private L4D2MM Manager { get; set; }
            public string Key { get; private set; }
            public ModState ModState { get; set; }
            public ModSource Source { get; private set; }
            public L4D2Mod Mod { get; private set; }

            public bool IsIgnoreCollision { get { return Manager.IgnoreList.ContainsKey(Key); } }
            public bool IsHaveCollision { get; private set; }

            public bool CanSetOn => ModState == ModState.Off;
            public bool CanSetOff => ModState == ModState.On || ModState == ModState.Unregisted;
            public bool CanSubcribe => Source == ModSource.Workshop && ModState == ModState.Unsubscribed;
            public bool CanUnsubscribe => Source == ModSource.Workshop && (ModState == ModState.On || ModState == ModState.Off || ModState == ModState.Unregisted);
            public bool CanDelete => Source == ModSource.Player;

            private bool Switch(bool check, ModState t)
            {
                if (!check)
                    return false;
                Manager.ChangeModState(Key, t);
                if (CanSetOff && !IsIgnoreCollision)
                    base.Regist();
                else if (CanSetOn || CanSubcribe)
                    base.Unregist();
                return true;
            }

            public bool SetOn() { return Switch(CanSetOn, ModState.On); }
            public bool SetOff() { return Switch(CanSetOff, ModState.Off); }
            public bool Subscribe() { return Switch(CanSubcribe, ModState.On); }
            public bool Unsubscribe() { return Switch(CanUnsubscribe, ModState.Unsubscribed); }
            public bool IgnoreCollision() { base.Unregist(); return Manager.IgnoreModCollision(Key); }
            public bool DetectCollision() { base.Regist(); return Manager.DetectModCollision(Key); }
            public bool Delete() { base.Unregist(); return Manager.DeleteMod(Key); }
        };

        Dictionary<string, ModInfo> m_modStates;
        public Dictionary<string, ModInfo> Mods { get { return m_modStates; } }

        CSSTL.Set<string> m_ignoreList;
        public CSSTL.Set<string> IgnoreList { get { return m_ignoreList; } }

        public L4D2MM()
        {
            m_steam = null;
            m_modStates = new Dictionary<string, ModInfo>();
            LoadIgnoreList();
        }

        public void Dispose()
        {
            SaveIgnoreList();
        }

        private void LoadIgnoreList()
        {
            if (File.Exists(m_ignoreFile))
            {
                string read = File.ReadAllText(m_ignoreFile, Encoding.UTF8);
                m_ignoreList = Newtonsoft.Json.JsonConvert.DeserializeObject<CSSTL.Set<string>>(read);
            }
            if(m_ignoreList == null)
            {
                m_ignoreList = new CSSTL.Set<string>();
            }
        }

        public void SaveIgnoreList()
        {
            File.WriteAllText(m_ignoreFile, Newtonsoft.Json.JsonConvert.SerializeObject(m_ignoreList), Encoding.UTF8);
            Logging.Log("save ignore list to file");
        }

        public bool SetEnableSteam(bool enable)
        {
            if (m_steam != null && !enable)
                m_steam = null;
            else if (m_steam == null && enable)
            {
                m_steam = new Facepunch.Steamworks.Client(m_appid);
                if (m_steam.SteamId <= 0)
                {
                    m_steam = null;
                    WindowCallbacks.Print(StringAdapter.GetInfo("LinkToSteamFailed"));
                }
                else
                {
                    WindowCallbacks.Print(StringAdapter.GetInfo("LinkToSteamUser") + " : " + m_steam.Username);
                    //LoadSteamWorkshop();
                }
            }
            return m_steam != null;
        }

        private void LoadSteamWorkshop()
        {
            if(m_steam != null)
            {
                WindowCallbacks.OperationEnable(this.GetType().ToString(), false);
                WindowCallbacks.Print(StringAdapter.GetInfo("LoadSteamWorkshop"));
                WindowCallbacks.Process(0, 1, StringAdapter.GetInfo("LoadSteamWorkshop"));
                var query = m_steam.Workshop.CreateQuery();
                var list = m_steam.Workshop.GetSubscribedItemIds();
                {
                    query.FileId = list.ToList();
                    query.Run();
                    query.Block();
                    Logging.Assert(!query.IsRunning);
                    Logging.Assert(query.TotalResults > 0);
                    Logging.Assert(query.Items.Length > 0);
                }
                int total = query.Items.Length;
                int count = 0;
                foreach (var item in query.Items)
                {
                    ulong id = item.Id;
                    //Facepunch.Steamworks.Workshop.Item item = m_steam.Workshop.GetItem(v);
                    if (id > 0)
                    {
                        WindowCallbacks.Process(count++, total, StringAdapter.GetInfo("LoadSteamWorkshop") + " : " + id.ToString());
                        string key = @"workshop\" + id.ToString() + ".vpk";
                        if (!m_modStates.ContainsKey(key))
                        {
                            L4D2Mod mod = new L4D2Mod(item);
                            if (mod.Title != null && mod.Title.Length > 0)
                                m_modStates.Add(key, new ModInfo(this, key, ModState.On, ModSource.Workshop, mod));
                        }
                        else
                        {
                            m_modStates[key].Mod.LoadItem(item);
                        }
                        Logging.Log("load from Steam workshop " + id.ToString());
                    }
                }
                WindowCallbacks.Process(total - 1, total, "");
                WindowCallbacks.Print(StringAdapter.GetInfo("LoadComplete"));
                WindowCallbacks.OperationEnable(this.GetType().ToString(), true);
            }
        }

        private void LoadLocalFiles(string path)
        {
            WindowCallbacks.OperationEnable(this.GetType().ToString(), false);
            WindowCallbacks.Print(StringAdapter.GetInfo("LoadLocalFile"));
            int total = new DirectoryInfo(path + m_dirWorkshop).GetFiles("*.vpk").Length;
            if(Configure.EnableAddons)
                total += new DirectoryInfo(path + m_dirAddons).GetFiles("*.vpk").Length;
            int current = 0;
            foreach (FileInfo vpkFile in new DirectoryInfo(path + m_dirWorkshop).GetFiles("*.vpk")) //from workshop
            {
                //update process bar
                WindowCallbacks.Process(current, total, StringAdapter.GetInfo("LoadLocalFile") + " : workshop/" + vpkFile.Name);
                string key = @"workshop\" + vpkFile.Name;
                if (!m_modStates.ContainsKey(key))
                {
                    continue; //local file not valid in this path
                    L4D2Mod mod = new L4D2Mod(vpkFile, true);
                    ModInfo info = new ModInfo(this, key, ModState.On, ModSource.Workshop, mod);
                    m_modStates.Add(key, info);
                }
                else
                {
                    m_modStates[key].Mod.LoadLocalFile(vpkFile, Configure.EnableReadVpk);
                }
                current++;
                Logging.Log("load from workshop : " + vpkFile.Name);
            }
            //for addons
            if (Configure.EnableAddons)
                foreach (FileInfo vpkFile in new DirectoryInfo(path + m_dirAddons).GetFiles("*.vpk")) //from addons
                {

                    //update process bar
                    WindowCallbacks.Process(current, total, StringAdapter.GetInfo("LoadLocalFile") + " : " + vpkFile.Name);
                    string key = vpkFile.Name;
                    L4D2Mod mod = new L4D2Mod(vpkFile, true);
                    ModInfo info = new ModInfo(this, key, ModState.On, ModSource.Player, mod);
                    m_modStates.Add(key, info);
                    current++;
                    Logging.Log("load from addons : " + vpkFile.Name);
                }
            WindowCallbacks.Process(total, total, StringAdapter.GetInfo("LoadComplete"));
            WindowCallbacks.Print(StringAdapter.GetInfo("LoadComplete"));
            WindowCallbacks.OperationEnable(this.GetType().ToString(), true);
        }

        private string DetectConfig()
        {
            try //from registry
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(m_registryPath);
                string path = new FileInfo(registryKey.GetValue(m_registryKey).ToString().Split('\"')[1]).DirectoryName;
                Logging.Log("find path from registry : " + path);
                WindowCallbacks.Print(StringAdapter.GetInfo("FindPath") + " : " + path);
                return path;
            }
            catch
            {
                //can not get path from registry
                //try get it from cache
                if (Configure.InstallPath != "")
                {
                    Logging.Log("find path from cache file : " + Configure.InstallPath);
                    WindowCallbacks.Print(StringAdapter.GetInfo("FindPath") + " : " + Configure.InstallPath);
                    return Configure.InstallPath;
                }
            }
            return null;
        }

        public void Clear()
        {
            m_modStates.Clear();
        }

        public bool LoadConfig()
        {
            string path = Configure.InstallPath;
            Configure.InstallPath = ""; //reset path
            if (path == "")
                path = DetectConfig(); //detect path automatically
            if (path != null) //valid path
            {
                if (!CheckConfig(path))
                    return false;
                Configure.InstallPath = path;
                m_path = path;
                m_modStates.Clear();
                return true;
            }
            return false; //need user call the set function
        }

        private bool CheckConfig(string path)
        {
            //check path valid
            if(path == null)
            {
                Logging.Log("invalid path");
                return false;
            }
            if (!Directory.Exists(path + m_dirCore))
            {
                Logging.Log("can not find main directory");
                return false;
            }
            if (!File.Exists(path + m_fileList))
            {
                Logging.Log("can not find addons list file");
                return false;
            }
            if (!Directory.Exists(path + m_dirAddons))
            {
                Logging.Log("can not find addons directory");
                return false;
            }
            return true;
        }

        public void Initialize()
        {
            Logging.Assert(m_path != null);
            Clear();
            DoInitialize(m_path);
        }

        private void DoInitialize(string path)
        { 
            if (Configure.EnableSteam && m_steam != null)
                LoadSteamWorkshop();
            LoadLocalFiles(path);

            //read the addons list file
            WindowCallbacks.OperationEnable(this.GetType().ToString(), false);
            WindowCallbacks.Print(StringAdapter.GetInfo("UpdateModState"));
            WindowCallbacks.Process(1, 1, StringAdapter.GetInfo("UpdateModState"));
            L4D2TxtReader reader = new L4D2TxtReader(new FileStream(path + m_fileList, FileMode.Open), L4D2TxtReader.TxtType.AddonList);
            ModInfo nullModeInfo = new ModInfo(this, null, ModState.Miss, ModSource.Player, null);
            nullModeInfo.ModState = ModState.Miss;
            //update state of mods
            foreach (var v in reader.Values)
            {
                if (m_modStates.ContainsKey(v.Key))
                    if (v.Value == "0")
                        //m_modStates[v.Key].ModState = (v.Value == "1" ? ModState.On : ModState.Off);
                        m_modStates[v.Key].SetOff();
                //else
                //    m_modStates.Add(v.Key, nullModeInfo);
            }
            WindowCallbacks.Process(1, 1, StringAdapter.GetInfo("LoadComplete"));
            WindowCallbacks.OperationEnable(this.GetType().ToString(), true);

            //foreach (var v in m_modStates)
                //Logging.Log(v.Key + " : \t" + v.Value.ModState.GetString());

            //download preview files
            new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                foreach (var v in m_modStates)
                {
                    var mod = v.Value.Mod;
                    if(v.Value.Source == ModSource.Workshop && mod.Image == null && mod.ImageURL != "")
                    {
                        try
                        {
                            mod.LoadPreviewImageFromURL(m_path + m_dirWorkshop);
                        }
                        catch { }
                    }
                }
            })).Start();

            Logging.Log("initialize success : " + path);
        }

        public bool DeleteMod(string key)
        {
            Logging.Assert(m_modStates.ContainsKey(key));
            ModInfo mod = m_modStates[key];
            if (mod.ModState != ModState.Miss)
            {
                foreach (var v in new DirectoryInfo(mod.Source == ModSource.Player ? m_dirAddons : m_dirWorkshop)
                    .GetFiles(mod.Mod.FileName.Substring(0, mod.Mod.FileName.LastIndexOf('.')) + ".*"))
                    v.Delete();
            }
            m_modStates.Remove(key);
            return true;
        }

        public bool IgnoreModCollision(string key)
        {
            if (m_ignoreList.Contains(key))
                return false;
            m_ignoreList.Add(key);
            return true;
        }

        public bool DetectModCollision(string key)
        {
            if (!m_ignoreList.Contains(key))
                return false;
            m_ignoreList.Remove(key);
            return true;
        }

        public void ChangeModState(string key, ModState state)
        {
            Logging.Assert(state == ModState.On || state == ModState.Off || state == ModState.Unsubscribed);
            Logging.Assert(m_modStates.ContainsKey(key));
            ModInfo mod = m_modStates[key];
            ModState oldState = mod.ModState;
            if (oldState != state)
            {
                if(oldState == ModState.Unsubscribed)
                    m_steam.Workshop.GetItem(mod.Mod.PublishedId).Subscribe();
                if (state == ModState.Unsubscribed)
                {
                    Logging.Assert(m_steam != null);
                    Logging.Assert(mod.Mod.PublishedId > 0);
                    m_steam.Workshop.GetItem(mod.Mod.PublishedId).UnSubscribe();
                }
                mod.ModState = state;
            }
        }

        public bool SaveModState()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"AddonList\"\n{\n");
                foreach (var v in m_modStates)
                    if (v.Value.ModState == ModState.On || v.Value.ModState == ModState.Off)
                        sb.Append("\t\"" + v.Key + "\"\t\t\"" + (v.Value.ModState == ModState.On ? '1' : '0') + "\"\n");
                sb.Append("}\n");
                File.WriteAllText(m_path + '\\' + m_fileList, sb.ToString());
                Logging.Log("save mod state to file");
            }
            catch (Exception e)
            {
                Logging.Log(e.Message, "CATCH");
                return false;
            }
            return true;
        }
    }
}
