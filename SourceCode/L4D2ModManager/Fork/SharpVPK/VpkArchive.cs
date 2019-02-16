using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpVPK.Exceptions;
using SharpVPK.V1;

namespace SharpVPK
{
    public class VpkArchive
    {
        public List<VpkDirectory> Directories { get; set; }
        public bool IsMultiPart { get; set; }
        private VpkReaderBase reader;
        internal List<ArchivePart> Parts { get; set; }
        internal string ArchivePath { get; set; }
        public uint ArchiveOffset { get; private set; }

        public VpkArchive()
        {
            Directories = new List<VpkDirectory>();
        }

        public void Load(string filename)
        {
            ArchivePath = filename;
            IsMultiPart = filename.EndsWith("dir.vpk");
            if (IsMultiPart)
                LoadParts(filename);
            reader = new VpkReaderV1(filename);
            var hdr = reader.ReadArchiveHeader();
            if (!hdr.Verify())
                throw new ArchiveParsingException("Invalid archive header");
            ArchiveOffset = hdr.CalculateDataOffset();
            Directories.AddRange(reader.ReadDirectories(this));
        }

        private void LoadParts(string filename)
        {
            Parts = new List<ArchivePart>();
            var fileBaseName = filename.Split('_')[0];
            foreach (var file in Directory.GetFiles(Path.GetDirectoryName(filename)))
            {
                if (file.Split('_')[0] != fileBaseName || file == filename)
                    continue;
                var fi = new FileInfo(file);
                var partIdx = Int32.Parse(file.Split('_')[1].Split('.')[0]);
                Parts.Add(new ArchivePart((uint)fi.Length, partIdx, file));
            }
            Parts.Add(new ArchivePart((uint) new FileInfo(filename).Length, -1, filename));
            Parts = Parts.OrderBy(p => p.Index).ToList();
        }

        public void MergeDirectories()
        {
            Dictionary<string, VpkDirectory> cache = new Dictionary<string,VpkDirectory>();
            for (int i = 0; i < Directories.Count; )
            {
                if (cache.ContainsKey(Directories[i].Path))
                {
                    cache[Directories[i].Path].Entries.AddRange(Directories[i].Entries);
                    Directories.RemoveAt(i);
                }
                else
                {
                    cache.Add(Directories[i].Path, Directories[i]);
                    i++;
                }
            }
        }
    }
}
