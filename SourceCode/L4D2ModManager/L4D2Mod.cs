using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Text.RegularExpressions;

namespace L4D2ModManager
{
    class L4D2Mod : IDisposable
    {
        static string[] ImageExtensions = new string[] { "jpg", "png", "bmp" };

        public string FileName { get; private set; }
        public long FileSize { get; private set; }
        public ulong PublishedId { get; private set; }
        public HashSet<L4D2Type.Category> Category { get { return m_category; } }
        public Image Image { get; private set; }
        public MemoryStream ImageMemoryStream { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }
        public string[] Tags { get; private set; }
        public string ImageURL { get; private set; }
        public string Version { get; private set; }
        public string URL { get; private set; }
        public ulong OwnerId { get; private set; }
        public float Score { get; private set; }

        private HashSet<L4D2Type.Category> m_category;
        static public Dictionary<string, L4D2Type.Category> FastCategoryMatchMap = new Dictionary<string, L4D2Type.Category>();
        private List<KeyValuePair<string, Action<L4D2Mod, SharpVPK.VpkDirectory>>> m_responces;

        public MemoryStream GetAndResetImageMemoryStream()
        {
            ImageMemoryStream.Seek(0, SeekOrigin.Begin);
            return ImageMemoryStream;
        }

        protected void DoInitialize()
        {
            FileName = "";
            FileSize = 0;
            PublishedId = 0;
            m_category = new HashSet<L4D2Type.Category>();
            ImageMemoryStream = null;
            Image = null;
            Title = "";
            Author = "";
            Description = "";
            ImageURL = "";
            Version = "";
            URL = "";
            OwnerId = 0;
            Score = -1;
        }

        public void Dispose()
        {
            ImageMemoryStream?.Dispose();
            Image?.Dispose();
        }

        public L4D2Mod(string json, string description)
        {
            DoInitialize();
            LoadJson(json, description);
        }

        public void LoadJson(string json, string description)
        {
            Newtonsoft.Json.Linq.JObject j = Newtonsoft.Json.Linq.JObject.Parse(json);
            foreach (var v in j)
            {
                var prop = this.GetType().GetProperty(v.Key);
                prop.SetValue(this, v.Value.ToObject(prop.PropertyType));
                if (v.Key == "Tags")
                    Tags = Tags[0].ToLower().Split(',');
                else if(v.Key == "PublishedId")
                    FileName = PublishedId.ToString() + ".vpk";
            }
            Description = description;
            HandleTags();
        }

        public bool LoadItem(Facepunch.Steamworks.Workshop.Item item)
        {
            Logging.Assert(item.Id != 0);
            if(item.Id <= 0)
                return false;
            FileName = item.Id.ToString() + ".vpk";
            FileSize = (long)item.FileSize;
            PublishedId = item.Id;
            ImageURL = item.PreviewImageUrl;
            //Author = item.OwnerName;//some problem here, author is not included in firend list, need refine it with RequestUserStats
            if (true) //need decoding
            {
                Title = item.Title;
                Description = item.Description;
                Tags = item.Tags;
            }
            else // from URL : takes a long time
            {
                string url = WebHelper.GetWebClient(item.Url);
                url = url.Substring(0, url.IndexOf(@"<link"));
                Match match = Regex.Match(url, @"<meta.*content=""([\s\S]*)"">[\s\S]*<title>(.+)</title>", RegexOptions.Multiline);
                Logging.Assert(match.Success, url);

                if (match.Groups.Count < 3)
                    return false;
                if (!match.Groups[1].Value.Contains(':'))
                    return false;
                if (!match.Groups[2].Value.Contains("::"))
                    return false;
                Description = match.Groups[1].Value.Substring(match.Groups[1].Value.IndexOf(':') + 1);
                Title = match.Groups[2].Value.Substring(match.Groups[2].Value.IndexOf("::") + 1);
            }
            HandleTags();
            return true;
        }

        public void LoadPreviewImageFromURL(string path, bool enableCache = true)
        {
            string filename = path + FileName.Replace(".vpk", ".jpg");
            if(!enableCache || !System.IO.File.Exists(filename))
            {
                var web = new System.Net.WebClient();
                web.DownloadFile(ImageURL, filename);
            }
            ReadImageFromFile(filename);
        }

        private void ReadImageFromFile(string filename)
        {
            var fileinfo = new FileInfo(filename);
            if (fileinfo.Exists)
            {
                try
                {
                    Image = System.Drawing.Image.FromFile(filename);
                    var bitmap = new System.Drawing.Bitmap(Image);
                    ImageMemoryStream = new MemoryStream();
                    bitmap.Save(ImageMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (Exception e)
                {

                    if (fileinfo.Length <= 0)
                        fileinfo.Delete();
                    Image = null;
                    ImageMemoryStream = null;
                    Logging.Log("Error in reading file : " + filename + ", " + e.Message + "\r\n" + e.Source + "\r\n" + e.StackTrace, "CATCH");
                }
            }
        }
        
        private void HandleTags()
        {
            if(Tags != null && Tags.Length > 0)
            {
                foreach (var v in Tags)
                {
                    var tag = v.ToLower();
                    if (FastCategoryMatchMap.ContainsKey(tag))
                    {
                        if(FastCategoryMatchMap[tag] != null)
                            Category.Add(FastCategoryMatchMap[tag]);
                    }
                    else
                    {
                        var match = L4D2Type.Match(v);
                        if(match != null)
                            Category.Add(match);
                        FastCategoryMatchMap.Add(v, match);
                    }
                }
            }
        }

        public L4D2Mod(Facepunch.Steamworks.Workshop.Item item)
        {
            DoInitialize();
            LoadItem(item);
        }

        public void LoadLocalFile(FileInfo file, bool enableVpk)
        {
            //find image
            foreach (var ext in ImageExtensions)
            {
                var filename = file.FullName.Replace(".vpk", '.' + ext);
                if (File.Exists(filename))
                {
                    ReadImageFromFile(filename);
                    break;
                }
            }

            FileName = file.Name;
            FileSize = file.Length;

            if (enableVpk)
            {
                m_responces = new List<KeyValuePair<string, Action<L4D2Mod, SharpVPK.VpkDirectory>>>();
                AppendResponce(" ", HandleRoot);
                AppendResponce("models/prop.*", HandleProps);
                AppendResponce("models/infected", HandleInfected);
                AppendResponce("models/survivors", HandleSurvivor);
                AppendResponce("models/v_models", HandleModelVItem);
                AppendResponce(".*", HandleOthers);

                //deal with information from vpk or directory
                var vpk = new SharpVPK.VpkArchive();
                vpk.Load(file.FullName);
                vpk.MergeDirectories();
                foreach (var directory in vpk.Directories)
                {
                    if (directory.Path.Length > 0)
                    {
                        foreach (var regex in m_responces)
                        {
                            if (Regex.Match(directory.Path, regex.Key).Length == directory.Path.Length)
                            {
                                //Logging.Log("load " + vpk.ArchivePath.Split('/').Last().Split('\\').Last() + " , directory : " + directory.Path + " matches " + regex.Key.ToString());
                                regex.Value.Invoke(this, directory);
                                break;
                            }
                        }
                    }
                }

                var regexHandle = new L4D2Type.RegexClassifierHandle();
                //regexHandle.HandleRegex(FileName);
                regexHandle.HandleRegex("<title>" + Title);
                regexHandle.HandleRegex("<author>" + Author);
                foreach (var directory in vpk.Directories)
                {
                    if (directory.Path.Length > 0)
                    {
                        foreach(var entry in directory.Entries)
                        {
                            var name = directory.Path + '/' + entry.Filename + '.' + entry.Extension;
                            regexHandle.HandleRegex(name);
                            if (regexHandle.Completed)
                                goto RegexEnd;
                        }
                    }
                }
                RegexEnd:
                try
                {
                    var regexResults = regexHandle.Match();
                    foreach (var v in regexResults)
                    {
                        if (FastCategoryMatchMap.ContainsKey(v))
                        {
                            if (FastCategoryMatchMap[v] != null)
                                Category.Add(FastCategoryMatchMap[v]);
                        }
                        else
                        {
                            var match = L4D2Type.Match(v);
                            if (match != null)
                                Category.Add(match);
                            FastCategoryMatchMap.Add(v, match);
                        }
                    }
                }
                catch (L4D2Type.SyntaxError e)
                {
                    Logging.Log(e.Message, "Syntax Error");
                }
            }
        }

        public L4D2Mod(FileInfo file, bool enableVpk)
        {
            DoInitialize();
            LoadLocalFile(file, enableVpk);
        }

        void AppendResponce(string regex, Action<L4D2Mod, SharpVPK.VpkDirectory> responce)
        {
            m_responces.Add(new KeyValuePair<string, Action<L4D2Mod, SharpVPK.VpkDirectory>>(regex, responce));
        }

        static void HandleRoot(L4D2Mod o, SharpVPK.VpkDirectory dir)
        {
            bool isFindImage = o.ImageMemoryStream != null;
            foreach (var entry in dir.Entries)
            {
                if (!isFindImage && entry.Filename == "addonimage")
                {
                    if (ImageExtensions.Contains(entry.Extension))
                    {
                        o.ImageMemoryStream = new MemoryStream();
                        byte[] data = entry.Data;
                        o.ImageMemoryStream.Write(data, 0, data.Length);
                        o.Image = Image.FromStream(o.ImageMemoryStream);
                        isFindImage = true;
                        o.Image.Save(dir.ParentArchive.ArchivePath.Replace(".vpk", '.' + entry.Extension));
                    }
                }
                if (entry.Filename == "addoninfo" && entry.Extension == "txt")
                {
                    HandleAddonInfoTxt(o, entry);
                    //Logging.Error();
                }
            }
        }

        static void HandleAddonInfoTxt(L4D2Mod o, SharpVPK.VpkEntry entry)
        {
            bool AddDescription = o.Description.Length <= 0;
            MemoryStream ms = new MemoryStream();
            byte[] data = entry.Data;
            ms.Write(data, 0, data.Length);
            ms.Seek(0, SeekOrigin.Begin);
            L4D2TxtReader reader = new L4D2TxtReader(ms, L4D2TxtReader.TxtType.AddonInfo);
            foreach(var item in reader.Values)
            {
                var key = item.Key.ToLower();
                if (key.Contains("title") && o.Title.Length <= 0)
                    o.Title = item.Value.Replace('\r', ' ').Replace('\n', ' ');
                else if (key.Contains("author") && o.Author.Length <= 0)
                    o.Author = item.Value;
                else if (key.Contains("description") && AddDescription)
                    o.Description = o.Description + item.Value + "\r\n";
                else if (key.Contains("version") && o.Version.Length <= 0)
                    o.Version = item.Value;
                else if (key.Contains("url") && o.URL.Length <= 0)
                    o.URL = item.Value;
                else if (key.Contains("ownerid") && o.OwnerId <= 0)
                {
                    try { o.OwnerId = Convert.ToUInt64(item.Value); }
                    catch { }
                }
            }
        }

        static void HandleProps(L4D2Mod o, SharpVPK.VpkDirectory dir)
        {

        }

        static void HandleInfected(L4D2Mod o, SharpVPK.VpkDirectory dir)
        {

        }

        static void HandleSurvivor(L4D2Mod o, SharpVPK.VpkDirectory dir)
        {

        }

        static void HandleModelVItem(L4D2Mod o, SharpVPK.VpkDirectory dir)
        {

        }

        static void HandleOthers(L4D2Mod o, SharpVPK.VpkDirectory dir)
        {

        }
    }
}
