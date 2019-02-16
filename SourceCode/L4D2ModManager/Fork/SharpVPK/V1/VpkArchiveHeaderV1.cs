namespace SharpVPK.V1
{
    public struct VpkArchiveHeaderV1 : IVpkArchiveHeader
    {
        private const int StaticSignature = 0x55aa1234;

        public uint Signature { get; set; }
        public uint Version { get; set; }
        public uint TreeLength { get; set; }

        public VpkArchiveHeaderV1(uint signature, uint version, uint treeLength)
            : this()
        {
            Signature = signature;
            Version = version;
            TreeLength = treeLength;
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
