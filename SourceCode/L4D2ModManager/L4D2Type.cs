using System;
using System.Collections.Generic;

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

    static class L4D2Type
    {
        class Storager
        {
            public Dictionary<ModCategory, object[]> Stored { get; private set; }
            public Storager()
            {
                Stored = new Dictionary<ModCategory, object[]>();
                Stored.Add(ModCategory.Survivors, GetEnumValues<SurvivorCategory>());
                Stored.Add(ModCategory.Infected, GetEnumValues<InfectedCategory>());
            }
            static private object[] GetEnumValues<T>()
            {
                var array = Enum.GetValues(typeof(T));
                object[] objs = new object[array.Length];
                array.CopyTo(objs, 0);
                return objs;
            }
        }
        static private Storager StoragerInstance = new Storager();

        public static object[] GetSubcategory(ModCategory category)
        {
            if (StoragerInstance.Stored.ContainsKey(category))
                return StoragerInstance.Stored[category];
            return null;
        }
    }
}
