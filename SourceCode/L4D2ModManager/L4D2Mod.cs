using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace L4D2ModManager
{
    class L4D2Mod
    {
        static string[] ImageExtensions = new string[] { "jpg", "png", "bmp" };

        public string FileName { get; private set; }
        public long FileSize { get; private set; }
        public ulong PublishedId { get; set; }
        public ModCategory[] Category { get { return m_category.Keys.ToArray(); } }
        public object[] SubCategory(ModCategory c) { return m_category[c].Keys.ToArray(); }
        public Image Image { get; private set; }
        public MemoryStream ImageMemoryStream { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }
        public string[] Tags { get; private set; }
        public string ImageURL { get; private set; }

        private Dictionary<ModCategory, CSSTL.Set<object>> m_category;
        static private Dictionary<string, KeyValuePair<ModCategory, object>> FastCategoryMatchMap = new Dictionary<string, KeyValuePair<ModCategory, object>>();
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
            m_category = new Dictionary<ModCategory, CSSTL.Set<object>>();
            ImageMemoryStream = null;
            Image = null;
            Title = "";
            Author = "";
            Description = "";
            ImageURL = "";
        }

        public bool LoadItem(Facepunch.Steamworks.Workshop.Item item)
        {
            Logging.Assert(item.Id != 0);
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

        public void LoadPreviewImageFromURL(string path)
        {
            string filename = path + FileName.Replace(".vpk", ".jpg");
            var web = new System.Net.WebClient();
            web.DownloadFile(ImageURL, filename);
            Image = System.Drawing.Image.FromFile(filename);
            var bitmap = new System.Drawing.Bitmap(Image);
            ImageMemoryStream = new MemoryStream();
            bitmap.Save(ImageMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        
        private void HandleTags()
        {
            if(Tags != null && Tags.Length > 0)
            {
                Action<KeyValuePair<ModCategory, object>> AddCategory = (ret) =>
                {
                    if (!m_category.ContainsKey(ret.Key))
                        m_category.Add(ret.Key, new CSSTL.Set<object>());
                    if (ret.Value != null)
                        m_category[ret.Key].TryAdd(ret.Value);
                };
                Action<string, ModCategory, object> AddCategoryAndStore = (tag, category, o) =>
                {
                    var result = new KeyValuePair<ModCategory, object>(category, o);
                    FastCategoryMatchMap.Add(tag, result);
                    AddCategory(result);
                };
                foreach (var v in Tags)
                {
                    var tag = v.ToLower();
                    if (FastCategoryMatchMap.ContainsKey(tag))
                    {
                        AddCategory(FastCategoryMatchMap[tag]);
                    }
                    else
                    {
                        foreach(var e in Enum.GetValues(typeof(ModCategory)))
                        {
                            var array = L4D2Type.GetSubcategory((ModCategory)e);
                            if(array != null)
                            {
                                foreach(var o in array)
                                {
                                    if(tag.Equals(o.ToString().ToLower()))
                                    {
                                        AddCategoryAndStore(tag, (ModCategory)e, o);
                                        goto a_nextTag;
                                    }
                                }
                            }
                            else
                            {
                                if(tag.Equals(e.ToString().ToLower()))
                                {
                                    AddCategoryAndStore(tag, (ModCategory)e, null);
                                    goto a_nextTag;
                                }
                            }
                        }
                        //can not find... give some special rules here
                        if(tag == "common infected")
                            AddCategoryAndStore(tag, ModCategory.Infected, InfectedCategory.Common);
                        else if(tag == "special infected")
                            AddCategoryAndStore(tag, ModCategory.Infected, null);
                    }
                    a_nextTag:;
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
                    Image = Image.FromFile(filename);
                    Bitmap bitmap = new Bitmap(Image);
                    ImageMemoryStream = new MemoryStream();
                    bitmap.Save(ImageMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
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
                                Logging.Log("load " + vpk.ArchivePath.Split('/').Last().Split('\\').Last() + " , directory : " + directory.Path + " matches " + regex.Key.ToString());
                                regex.Value.Invoke(this, directory);
                                break;
                            }
                        }
                    }
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
            MemoryStream ms = new MemoryStream();
            byte[] data = entry.Data;
            ms.Write(data, 0, data.Length);
            ms.Seek(0, SeekOrigin.Begin);
            L4D2TxtReader reader = new L4D2TxtReader(ms, L4D2TxtReader.TxtType.AddonInfo);
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
