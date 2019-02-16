namespace SharpVPK.V2
{
    public struct VpkArchiveHeaderV2 : IVpkArchiveHeader
    {
        private const int StaticSignature = 0x55aa1234;

        public uint Signature { get; set; }
        public uint Version { get; set; }
        public uint TreeLength { get; set; }
        public uint FooterLength { get; set; }

        public VpkArchiveHeaderV2(uint signature, uint version, uint treeLength, uint footerLength)
            : this()
        {
            Signature = signature;
            Version = version;
            TreeLength = treeLength;
            FooterLength = footerLength;
        }

        public bool Verify()
        {
            return Signature == StaticSignature && Version == 1;
        }

        public uint CalculateDataOffset()
        {
            return 12 + TreeLength;
        }
    }
}
