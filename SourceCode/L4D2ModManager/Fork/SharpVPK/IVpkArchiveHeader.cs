using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpVPK
{
    interface IVpkArchiveHeader
    {
        uint Signature { get; set; }
        uint Version { get; set; }
        uint TreeLength { get; set; }

        bool Verify();
        uint CalculateDataOffset();
    }
}
