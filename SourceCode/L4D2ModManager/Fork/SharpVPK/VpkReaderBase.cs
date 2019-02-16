using L4D2ModManager;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpVPK
{
    internal abstract class VpkReaderBase
    {
        public BinaryReader Reader;
        private readonly StringBuilder strBuilder;

        protected VpkReaderBase(string filename)
        {
            Reader = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            strBuilder = new StringBuilder(256);
        }

        public abstract IVpkArchiveHeader ReadArchiveHeader();

        public string ReadNullTerminatedString()
        {
            strBuilder.Clear();
            char chr;
            while ((chr = (char)Reader.ReadByte()) != 0x0)
                strBuilder.Append(chr);
            return strBuilder.ToString();
        }

        protected T BytesToStructure<T>(byte[] bytearray)
        {
            var structSize = Marshal.SizeOf(typeof (T));
            var pStruct = Marshal.AllocHGlobal(structSize);
            Marshal.Copy(bytearray, 0, pStruct, structSize);
            var @struct = (T)Marshal.PtrToStructure(pStruct, typeof(T));
            Marshal.FreeHGlobal(pStruct);

            return @struct;
        }

        public IEnumerable<VpkDirectory> ReadDirectories(VpkArchive parentArchive)
        {
            while (true)
            {
                var ext = ReadNullTerminatedString();
                if (string.IsNullOrEmpty(ext))
                    break;
                while (true)
                {
                    var path = ReadNullTerminatedString();
                    if (string.IsNullOrEmpty(path))
                        break;

                    var entries = ReadEntries(parentArchive, ext, path).ToList();
                    yield return new VpkDirectory(parentArchive, path, entries);
                }
            }
        }

        public IEnumerable<VpkEntry> ReadEntries(VpkArchive parentArchive, string ext, string path)
        {
            while (true)
            {
                var fileName = ReadNullTerminatedString();
                if (string.IsNullOrEmpty(fileName))
                    break;

                var crc = Reader.ReadUInt32();
                var preloadBytes = Reader.ReadUInt16();
                var archiveIdx = Reader.ReadUInt16();
                var entryOffset = Reader.ReadUInt32();
                var entryLen = Reader.ReadUInt32();
                // skip terminator
                if (Reader.ReadUInt16() != 0xFFFF)
                    throw new FileFormatException();

                var preloadDataOffset = (uint)Reader.BaseStream.Position;
                if (preloadBytes > 0) 
                    Reader.BaseStream.Position += preloadBytes;

                yield return new VpkEntry(parentArchive, crc, preloadBytes, preloadDataOffset, archiveIdx, entryOffset, entryLen, ext, path, fileName);
            }
        }

        public abstract uint CalculateEntryOffset(uint offset);
    }
}

