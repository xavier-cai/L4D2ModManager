using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpVPK
{
    public class VpkEntry
    {
        public string Extension { get; set; }
        public string Path { get; set; }
        public string Filename { get; set; }
        public byte[] PreloadData { get { return ReadPreloadData(); }}
        public byte[] Data { get { return ReadData(); } }
        public bool HasPreloadData { get; set; }

        internal uint CRC;
        internal ushort PreloadBytes;
        internal uint PreloadDataOffset;
        internal ushort ArchiveIndex;
        internal uint EntryOffset;
        internal uint EntryLength;
        internal VpkArchive ParentArchive;

        internal VpkEntry(VpkArchive parentArchive, uint crc, ushort preloadBytes, uint preloadDataOffset, ushort archiveIndex, uint entryOffset,
            uint entryLength, string extension, string path, string filename)
        {
            ParentArchive = parentArchive;
            CRC = crc;
            PreloadBytes = preloadBytes;
            PreloadDataOffset = preloadDataOffset;
            ArchiveIndex = archiveIndex;
            EntryOffset = entryOffset;
            EntryLength = entryLength;
            Extension = extension;
            Path = path;
            Filename = filename;
            HasPreloadData = preloadBytes > 0;
        }

        public override string ToString()
        {
            return string.Concat(Path, "/", Filename, ".", Extension);
        }

        private byte[] ReadPreloadData()
        {
            if (PreloadBytes > 0)
            {
                var buff = new byte[PreloadBytes];
                using (var fs = new FileStream(ParentArchive.ArchivePath, FileMode.Open, FileAccess.Read))
                {
                    buff = new byte[PreloadBytes];
                    fs.Seek(PreloadDataOffset, SeekOrigin.Begin);
                    fs.Read(buff, 0, buff.Length);
                }
                return buff;
            }
            return null;
        }

        private byte[] ReadData()
        {
            string partFile = ParentArchive.ArchivePath;
            if (ArchiveIndex != 0x7fff)
            {
                var file = ParentArchive.Parts.FirstOrDefault(part => part.Index == ArchiveIndex);
                if (file == null)
                    return null;
                partFile = file.Filename;
            }
            
            if (HasPreloadData)
                return ReadPreloadData();
            var buff = new byte[EntryLength];
            using (var fs = new FileStream(partFile, FileMode.Open, FileAccess.Read))
            {
                fs.Seek((ArchiveIndex == 0x7fff ? ParentArchive.ArchiveOffset : 0) + EntryOffset, SeekOrigin.Begin);
                fs.Read(buff, 0, buff.Length);
            }
            return buff;
        }

    }
}
