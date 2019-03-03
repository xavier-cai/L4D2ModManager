using System;
using System.Collections.Generic;
using System.Linq;

namespace L4D2ModManager
{
    class L4D2Resource
    {
        public abstract class ResourceHandler
        {
            public bool Registed { get; private set; } = false;
            Dictionary<ResourceItem, bool> m_resources = new Dictionary<ResourceItem, bool>();
            public Dictionary<ResourceItem, bool> Resources { get { return m_resources; } }
            public int CollisionCount { get; set; }
            public Action<int> CollisionCountChangeHandler = null;

            protected void ClearResource()
            {
                Unregist();
                m_resources.Clear();
            }

            protected void AddResource(ResourceItem item)
            {
                if (!m_resources.ContainsKey(item))
                    m_resources.Add(item, false);
            }

            /*
            protected void Regist()
            {
                Regist(item => true);
            }
            */

            protected void Regist(Predicate<ResourceItem> predicate)
            {
                Registed = true;
                var keys = Resources.Keys.ToArray();
                foreach (var v in keys)
                    if(predicate(v))
                        v.Regist(this);
            }

            protected void Unregist()
            {
                Registed = false;
                var keys = Resources.Keys.ToArray();
                foreach (var v in keys)
                    v.Unregist(this);
            }

            public void UpdateResource(ResourceItem item, bool state)
            {
                int change = 0;
                Logging.Assert(m_resources.ContainsKey(item));
                if(!m_resources.ContainsKey(item))
                {
                    m_resources.Add(item, state);
                    if (state)
                        change = 1;
                }
                else
                {
                    bool oldState = m_resources[item];
                    m_resources[item] = state;
                    if (oldState && !state)
                        change = -1;
                    else if (!oldState && state)
                        change = 1;
                }
                Logging.Assert(!(change == -1 && CollisionCount == 0));
                CollisionCount += change;
                if (change != 0 && CollisionCountChangeHandler != null)
                    CollisionCountChangeHandler(CollisionCount);
            }
        }

        public class ResourceItem
        {
            public object Identity { private set; get; }
            public string Display { private set; get; }
            public ResourceItem(object identity, string display)
            {
                Identity = identity;
                Display = display;
            }

            private CSSTL.Set<ResourceHandler> m_handlers = new CSSTL.Set<ResourceHandler>();

            public void Regist(ResourceHandler o)
            {
                if(m_handlers.TryAdd(o))
                {
                    if (m_handlers.Count == 2)
                        foreach (var v in m_handlers)
                            v.Key.UpdateResource(this, true);
                    else if (m_handlers.Count > 2)
                        o.UpdateResource(this, true);
                }
            }

            public void Unregist(ResourceHandler o)
            {
                if(m_handlers.Remove(o))
                {
                    o.UpdateResource(this, false);
                    if (m_handlers.Count == 1)
                        foreach (var v in m_handlers)
                            v.Key.UpdateResource(this, false);
                }
            }
        }

        Dictionary<object, ResourceItem> Storager;

        static private L4D2Resource Instance = new L4D2Resource();
        private L4D2Resource()
        {
            Storager = new Dictionary<object, ResourceItem>();
        }

        static public ResourceItem GetResource(L4D2Type.Category category)
        {
            if (Instance.Storager.ContainsKey(category))
                return Instance.Storager[category];
            var item = new ResourceItem(category, category.ToString());
            Instance.Storager.Add(category, item);
            return item;
        }

        static private string Solve(string name)
        {
            List<string> words = name.ToLower().Split('.')[0].Split('_').ToList();
            words.RemoveAll(w => new string[] { "v", "w", "eq", "anim", "survivor" }.Contains(w));
            return words.Aggregate((a, b) => { return a + ' ' + b; });
        }
    }
}
