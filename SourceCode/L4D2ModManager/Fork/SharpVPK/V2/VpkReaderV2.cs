using System.Runtime.InteropServices;
using SharpVPK.V1;

namespace SharpVPK.V2
{
    internal class VpkReaderV2 : VpkReaderBase
    {
        public VpkReaderV2(string filename) 
            : base(filename)
        {
        }

        public override IVpkArchiveHeader ReadArchiveHeader()
        {
            var hdrStructSize = Marshal.SizeOf(typeof(VpkArchiveHeaderV2));
            var hdrBuff = Reader.ReadBytes(hdrStructSize);
            // skip unknown values
            Reader.ReadInt32();
            var hdr = BytesToStructure<VpkArchiveHeaderV2>(hdrBuff);
            hdr.FooterLength = Reader.ReadUInt32();
            Reader.ReadInt32();
            Reader.ReadInt32();
            return hdr;
        }

        public override uint CalculateEntryOffset(uint offset)
        {
            throw new System.NotImplementedException();
        }
    }
}
