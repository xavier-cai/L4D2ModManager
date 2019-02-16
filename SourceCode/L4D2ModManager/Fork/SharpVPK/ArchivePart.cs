using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpVPK
{
    internal class ArchivePart
    {
        public uint Size { get; set; }
        public int Index { get; set; }
        public string Filename { get; set; }

        public ArchivePart(uint size, int index, string filename)
        {
            Size = size;
            Index = index;
            Filename = filename;
        }
    }
}
