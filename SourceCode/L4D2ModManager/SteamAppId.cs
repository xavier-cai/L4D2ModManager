using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace L4D2ModManager
{
    static class SteamAppId
    {
        static private string file = @"steamappid.txt";
        static private string[] path = {
                                           @"./",
                                           @"./bin/Debug/",
                                           @"./bin/Release/"
                                       };
        static public void SetAppId(uint id)
        {
            foreach (var p in path)
                if(Directory.Exists(p))
                    File.WriteAllText(p + file, id.ToString());
        }

    }
}
