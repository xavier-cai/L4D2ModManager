using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows.Threading;

namespace L4D2ModManager
{
    static class WinformAdapter
    {
        static public void ThreadSleep(uint miiliseconds)
        {
            System.Threading.Thread.Sleep((int)miiliseconds);
        }
    }
}
