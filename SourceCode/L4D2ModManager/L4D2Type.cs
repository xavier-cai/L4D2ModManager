using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace L4D2ModManager
{
    enum ModSource
    {
        Workshop,
        Player
    };

    enum ModCategory
    {
        Campaigns,
        Survivors,
        Scripts,
        Infected,
        Weapons,
        Items,
        Props,
        Miscellaneous,
        Others
    };

    enum SurvivorCategory
    {
        Bill,
        Francis,
        Louis,
        Zoey,
        Ellis,
        Coach,
        Nick,
        Rochelle,
    };

    enum InfectedCategory
    {
        Common,
        Uncommon,
        Boomer,
        Hunter,
        Smoker,
        Tank,
        Charger,
        Jockey,
        Spitter,
        Witch
    };

    public class Pair<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public Pair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    static class L4D2Type
    {
        public class Category
        {
            public string Name = "";
            public List<string> Keys = new List<string>();
            [Newtonsoft.Json.JsonIgnore]
            public Category Parent { get; private set; } = null;
            public List<Category> Children = new List<Category>();
            public int Level { get; private set; } = 0;
            public bool SingletonResource { get; set; } = false;

            private Category() { }

            public void RefreshChilrenTree()
            {
                foreach(var child in Children)
                {
                    child.Parent = this;
                    child.RefreshChilrenTree();
                }
            }

            public static Category CreateRoot()
            {
                Category root = new Category();
                root.Level = 0;
                return root;
            }

            public Category(string name)
            {
                Name = name;
            }

            public Category(Enum e)
            {
                Name = e.GetString();
            }

            public void AddChild(Category child)
            {
                Logging.Assert(child.Name.Length > 0);
                child.Level = Level + 1;
                child.Parent = this;
                Children.Add(child);
            }

            public bool RemoveChild(Category child)
            {
                if(Children.Remove(child))
                {
                    child.Parent = null;
                    return true;
                }
                return false;
            }

            public override string ToString()
            {
                if (Level.Equals(0))
                    return "[root]";
                if (Level.Equals(1))
                    return Name;
                return Parent.ToString() + '.' + Name;
            }
        }

        public sealed class SyntaxError : Exception
        {
            public SyntaxError(string msg)
                : base(msg)
            { }
        }

        internal class CustomContent
        {
            static string CustomCategoryFile = "category.ini";
            static string CustomRegexFile = "regex.ini";
            static string CustomClassifyFile = "classify.ini";
            public Category CategoryRoot; //include subcategory
            public List<Pair<string, string>> CustomRegex; //regex & regex key
            public List<Pair<string, string>> CustomClassify; //rule & target category key
            public CustomContent()
            {
                if (System.IO.File.Exists(CustomCategoryFile))
                {
                    CategoryRoot = Newtonsoft.Json.JsonConvert.DeserializeObject<Category>(System.IO.File.ReadAllText(CustomCategoryFile));
                    CategoryRoot.RefreshChilrenTree();
                }
                else
                    InitializeCategory();
                if (System.IO.File.Exists(CustomRegexFile))
                    CustomRegex = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Pair<string, string>>>(System.IO.File.ReadAllText(CustomRegexFile));
                else
                    InitializeCustomRegex();
                if (System.IO.File.Exists(CustomClassifyFile))
                    CustomClassify = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Pair<string, string>>>(System.IO.File.ReadAllText(CustomClassifyFile));
                else
                    InitializeCustomClassify();
            }
            public void SaveCustomContent()
            {
                Logging.Log("Saving category");
                System.IO.File.WriteAllText(CustomCategoryFile, Newtonsoft.Json.JsonConvert.SerializeObject(CategoryRoot));
                Logging.Log("Saving regexes");
                System.IO.File.WriteAllText(CustomRegexFile, Newtonsoft.Json.JsonConvert.SerializeObject(CustomRegex));
                Logging.Log("Saving classification rules");
                System.IO.File.WriteAllText(CustomClassifyFile, Newtonsoft.Json.JsonConvert.SerializeObject(CustomClassify));
            }
            void InitializeCategory()
            {
                Logging.Log("Custom : Initialize Category");
                CategoryRoot = new Category("");
                Dictionary<object, string[]> preload = new Dictionary<object, string[]>();
                preload.Add(SurvivorCategory.Bill, new string[] { "namvet" });
                preload.Add(SurvivorCategory.Zoey, new string[] { "teenangst" });
                preload.Add(SurvivorCategory.Rochelle, new string[] { "producer" });
                preload.Add(SurvivorCategory.Ellis, new string[] { "mechanic" });
                preload.Add(SurvivorCategory.Francis, new string[] { "biker" });
                preload.Add(SurvivorCategory.Louis, new string[] { "manager" });
                preload.Add(SurvivorCategory.Nick, new string[] { "gambler" });
                //preload.Add(SurvivorCategory.Coach, new string[] { "coach" });
                preload.Add(InfectedCategory.Common, new string[] { "common infected" });
                preload.Add(ModCategory.Infected, new string[] { "special infected" });
                preload.Add(InfectedCategory.Boomer, new string[] { "boomette" });
                preload.Add(InfectedCategory.Tank, new string[] { "hulk" });
                var AddPreload = new Action<Enum, Category>((e, c) =>
                {
                    if(preload.ContainsKey(e))
                    {
                        var keys = preload[e];
                        foreach (var key in keys)
                            c.Keys.Add(key);
                    }
                });
                var AddSubCategory = new Action<Type, L4D2Type.Category>((sub, category) =>
                {
                    foreach (var v in Enum.GetValues(sub))
                    {
                        var subcategory = new Category((Enum)v);
                        Logging.Assert(subcategory.Name.Length > 0);
                        subcategory.Keys.Add(v.ToString().ToLower());
                        AddPreload((Enum)v, subcategory);
                        subcategory.SingletonResource = true;
                        category.AddChild(subcategory);
                    }
                });
                foreach(var v in Enum.GetValues(typeof(ModCategory)))
                {
                    var e = (ModCategory)v;
                    var category = new Category(e);
                    category.Keys.Add(e.ToString().ToLower());
                    //category.SingletonResource = true;
                    AddPreload(e, category);
                    CategoryRoot.AddChild(category);
                    if (e == ModCategory.Survivors)
                        AddSubCategory(typeof(SurvivorCategory), category);
                    else if (e == ModCategory.Infected)
                        AddSubCategory(typeof(InfectedCategory), category);
                }
            }
            void InitializeCustomRegex()
            {
                Logging.Log("Custom : Initialize Regexes");
                CustomRegex = new List<Pair<string, string>>();
                CustomRegex.Add(new Pair<string, string>("Maps", @"maps/(?:.+)"));
                CustomRegex.Add(new Pair<string, string>("Scripts", @"cfg/autoexec\.cfg"));
                CustomRegex.Add(new Pair<string, string>("Survivor", @"models/survivors/survivor_(.+)\.mdl"));
                CustomRegex.Add(new Pair<string, string>("Infected", @"models/infected/(.+)\.mdl"));
                CustomRegex.Add(new Pair<string, string>("CommonInfected", @"models/infected/common(?:.*)\.mdl"));
                CustomRegex.Add(new Pair<string, string>("Weapons", @"models/weapons/(?:.+)\.mdl"));
            }
            void InitializeCustomClassify()
            {
                Logging.Log("Custom : Initialize Classification Rules");
                CustomClassify = new List<Pair<string, string>>();
                CustomClassify.Add(new Pair<string, string>("<Maps>", "campaigns"));
                CustomClassify.Add(new Pair<string, string>("<Scripts>", "scripts"));
                CustomClassify.Add(new Pair<string, string>("<Survivor>", "<Survivor.1>"));
                CustomClassify.Add(new Pair<string, string>("<Infected>", "<Infected.1>"));
                CustomClassify.Add(new Pair<string, string>("<CommonInfected>", "infected.common"));
                CustomClassify.Add(new Pair<string, string>("<Weapons> & !<Survivor>", "weapons"));
            }
        }

        static internal CustomContent CustomContentInstance = new CustomContent();

        internal static Category CategoryRoot => CustomContentInstance.CategoryRoot;

        public static List<Category> GetCategory()
        {
            return CustomContentInstance.CategoryRoot.Children;
        }

        private static HashSet<Category> Match(Category parent, string key, bool recursion = false, bool caseSensity = false)
        {
            if (!caseSensity)
                key = key.ToLower();
            HashSet<Category> ret = new HashSet<Category>();
            foreach (var child in parent.Children)
            {
                var compare = caseSensity ? child.Keys : child.Keys.Aggregate(new List<string>(), (list, k) => { list.Add(k.ToLower()); return list; });
                if (compare.Contains(key))
                    ret.Add(child);
                if (recursion)
                    ret.UnionWith(Match(child, key, true));
            }
            return ret;
        }

        public static Category Match(string match, bool caseSensity = false)
        {
            if (match.Length <= 0)
                return null;
            var keys = new List<string>(match.Split('.'));
            int level = -1;
            if(keys[0].IsNumber())
            {
                level = Convert.ToInt32(keys[0]);
                if (level == 1 || level == 2)
                    throw new SyntaxError("The given level [" + level.ToString() + "] is out of range.");
                keys.RemoveAt(0);
            }
            var matches = Match(CustomContentInstance.CategoryRoot, keys[0], true, caseSensity);
            matches.RemoveWhere(category =>
            {
                if (level != -1)
                    if (category.Level != level)
                        return true;
                return false;
            });
            for(int i = 1; i < keys.Count; i++)
            {
                if (matches.Count <= 0)
                    break;
                var ret = new HashSet<Category>();
                foreach(var parent in matches)
                    ret.UnionWith(Match(parent, keys[i], false, caseSensity));
                matches = ret;
            }
            var print = matches.Aggregate("", (s, c) => s += (',' + c.Name));
            Logging.Log("the command <" + match + "> matches " + (matches.Count > 0 ? " : " + print.Substring(1) : " nothing") + ".");
            return matches.Count > 0 ? matches.First() : null; 
        }



        public class RegexClassifierHandle
        {
            private static Dictionary<string, Regex> Regexes = new Dictionary<string, Regex>();
            private static Dictionary<string, string> RegexCache = new Dictionary<string, string>();
            private static RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;
            private static void UpdateRegex(string key, string regex)
            {
                if(!RegexCache.ContainsKey(key))
                {
                    RegexCache.Add(key, regex);
                    Regexes.Add(key, new Regex(regex, RegexOptions));
                    Logging.Log("Add regex [" + key + "] : " + regex);
                }
                else if (RegexCache[key] != regex)
                {
                    RegexCache[key] = regex;
                    Regexes[key] = new Regex(regex, RegexOptions);
                    Logging.Log("Update regex [" + key + "] : " + regex);
                }
            }
            private static void RefreshRegex()
            {
                if(Regexes.Count != CustomContentInstance.CustomRegex.Count)
                {
                    var clone = Regexes.Keys.ToArray();
                    foreach(var key in clone)
                    {
                        var find = CustomContentInstance.CustomRegex.Find(v => v.Key.Equals(key));
                        if(find == null)
                        {
                            Regexes.Remove(key);
                            RegexCache.Remove(key);
                            Logging.Log("Remove regex [" + key + "]");
                        }
                    }
                }
            }

            private Dictionary<string, bool> States;
            private Dictionary<string, List<Match>> Results;
            public bool Completed => States.Count == 0;
            private bool Hit(string name) => Results[name].Count > 0;

            public RegexClassifierHandle()
            {
                States = new Dictionary<string, bool>();
                Results = new Dictionary<string, List<Match>>();
                foreach (var regex in CustomContentInstance.CustomRegex)
                {
                    UpdateRegex(regex.Key, regex.Value);
                    States.Add(regex.Key, false);
                    Results.Add(regex.Key, new List<Match>());
                }
                RefreshRegex();
            }

            public void HandleRegex(string str)
            {
                if (Completed)
                    return;
                HashSet<string> matches = new HashSet<string>();
                foreach(var state in States)
                {
                    if(!state.Value)
                    {
                        var result = Regexes[state.Key].Match(str);
                        if(result.Success && result.Value.Equals(str))
                        {
                            //matches.Add(state.Key);
                            Results[state.Key].Add(result);
                        }
                    }
                }
                //remove completed item
                foreach (var match in matches)
                    States.Remove(match);
            }



            private bool PeekOne(List<bool> preResults, ref string command, ref int position, ref bool result)
            {
                result = false;
                for (; position < command.Length && command[position].Equals(' '); position++) ;
                if (position.Equals(command.Length) || command[position].Equals(')')) return false;
                result = true;
                switch (command[position])
                {
                    case '<':
                        var end = command.IndexOf('>', position);
                        if (end <= position)
                            throw new SyntaxError("Asymmetric struct : <>");
                        var name = command.Substring(position + 1, end - position - 1);
                        if (name.Length <= 0)
                            throw new SyntaxError("Empty value name");
                        position = end + 1;
                        if(name.IsNumber())
                        {
                            var preIndex = Convert.ToInt32(name) + 1;
                            if (preIndex < 0 || preIndex > preResults.Count)
                                throw new SyntaxError("Invalid index value : " + preIndex.ToString());
                            return preResults[preIndex - 1];
                        }
                        if (!Results.ContainsKey(name))
                            throw new SyntaxError("Can not find the value named : " + name);
                        return Hit(name);
                    case '(':
                        return Match(preResults, ref command, ref position);
                    case '!':
                        position++;
                        return !PeekOne(preResults, ref command, ref position, ref result);
                }
                throw new SyntaxError("unexcepted character '" + command[position] + "', position : " + position.ToString());
            }

            private bool PeekOperation (ref string command, ref int position, ref bool result)
            {
                result = false;
                for (; position < command.Length && command[position].Equals(' '); position++) ;
                if (position.Equals(command.Length) || command[position].Equals(')')) return false;
                result = true;
                char c = command[position++];
                switch (c)
                {
                    case '&': return true;
                    case '|': return false;
                }
                throw new SyntaxError("unexcepted character '" + c + "', position : " + (position - 1).ToString());
            }

            private bool Match(List<bool> preResults, ref string command, ref int position)
            {
                bool result = false; ;
                bool needBracket = false;
                if (command[position].Equals('('))
                {
                    position++;
                    needBracket = true;
                }
                var ret = PeekOne(preResults, ref command, ref position, ref result);
                if (!result)
                    throw new SyntaxError("Empty content in ()");
                do
                {
                    var operation = PeekOperation(ref command, ref position, ref result);
                    if (!result)
                        break;
                    var peek = PeekOne(preResults, ref command, ref position, ref result);
                    if (!result)
                        throw new SyntaxError("Missing operation value after the operation " + (operation ? '&' : '|'));
                    ret = operation ? ret && peek : ret || peek;
                } while (true);

                if((position.Equals(command.Length) && needBracket) || (position < command.Length && command[position].Equals(')') && !needBracket))
                    throw new SyntaxError("Asymmetric struct : ()");
                return ret;
            }

            public List<string> Match()
            {
                /*
                foreach(var result in Results)
                {
                    List<string[]> total = new List<string[]>();
                    Func<string[], string[], bool> Compare = (a, b) => a.Union(b).Except(a).Count().Equals(0);
                    Predicate<string[]> Delete = s => total.FindIndex(e => Compare(e, s)) >= 0;
                    List<Match> DeleteKeys = new List<Match>();
                    foreach (var match in result.Value)
                    {
                        string[] thisCapture = new string[match.Captures.Count];
                        int indexCapture = 0;
                        foreach (var v in match.Captures)
                            thisCapture[indexCapture++] = (v as Capture).Value;
                        if (Delete(thisCapture))
                            DeleteKeys.Add(match);
                        else
                            total.Add(thisCapture);
                    }
                    foreach (var delete in DeleteKeys)
                        result.Value.Remove(delete);
                }
                */
                var ret = new List<string>();
                var preResults = new List<bool>();
                foreach (var rule in CustomContentInstance.CustomClassify)
                {
                    try
                    {
                        string command = rule.Key;
                        int position = 0;
                        var match = Match(preResults, ref command, ref position);
                        preResults.Add(match);
                        if (match)
                        {
                            var convert = ConvertFromRegex(rule.Value);
                            ret.AddRange(convert);
                            Logging.Log("rule " + preResults.Count.ToString() + " matched, result : " + convert.Aggregate((a, b) => a + ", " + b));
                        }
                        else
                        {
                            Logging.Log("rule " + preResults.Count.ToString() + " matched nothing");
                        }
                    }
                    catch (SyntaxError e)
                    {
                        throw new SyntaxError(e.Message + ", error in handle the rule [Key: " + rule.Key + ", Value: " + rule.Value + "].");
                    }
                }
                return ret;
            }

            private static Regex ConvertRegex = new Regex(@"<.+?>", RegexOptions.Compiled);
            private List<string> ConvertFromRegex(string str)
            {
                List<string> ret = new List<string>() { str };
                var matches = ConvertRegex.Matches(str);
                var count = matches.Count;
                for(int index = count - 1; index >= 0; index--)
                {
                    var match = matches[index];
                    List<string> olds = ret;
                    ret = new List<string>();
                    var replaces = ConvertOneRegex(match.Value);
                    foreach(var old in olds)
                    {
                        var item = old.Remove(match.Index, match.Length);
                        foreach(var replace in replaces)
                        {
                            ret.Add(item.Insert(match.Index, replace));
                        }
                    }
                }
                return ret;
            }

            private HashSet<string> ConvertOneRegex(string str)
            {
                HashSet<string> ret = new HashSet<string>();
                var part = str.Substring(1, str.Length - 2);
                var splits = part.Split('.');
                if (splits.Length != 2)
                    throw new SyntaxError("The capture reference must contain two parts splited by '.', e.g <RegexName.CaptrueIndex>");
                if (!splits[1].IsNumber())
                    throw new SyntaxError("The capture index must be a number");
                var name = splits[0];
                if (!Results.ContainsKey(name))
                    throw new SyntaxError("Can not find the value named : " + name);
                if (!Hit(name))
                    throw new SyntaxError("Can not use a failed capture : " + name);
                var result = Results[name];
                var count = result[0].Groups.Count - 1;
                var index = Convert.ToInt32(splits[1]);
                if (index <= 0 || count < index)
                    throw new SyntaxError("Invalid captrue index value : " + index + ", which is excepted to locate in [1, " + count + "]");
                foreach (var v in result)
                    ret.Add(v.Groups[index].Value);
                return ret;
            }
        }
    }
}
