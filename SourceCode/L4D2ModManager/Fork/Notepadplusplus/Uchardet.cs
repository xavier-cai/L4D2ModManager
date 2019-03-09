using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Notepadplusplus
{
    public static class UcharDetector
    {
        private unsafe class Uchardet
        {
            [DllImport("uchardet.dll")] public static extern unsafe void Reset();
            [DllImport("uchardet.dll")] public static extern unsafe void HandleData(byte* buffer, uint size);
            [DllImport("uchardet.dll")] public static extern unsafe void HandleDataEnd();
            [DllImport("uchardet.dll")] public static extern unsafe void GetCharSet(System.Text.StringBuilder buffer, uint size);
        }

        private static unsafe string DetectImpl(Stream stream)
        {
            Uchardet.Reset();
            var streamPosition = stream.Position;
            var buffer = new byte[4096];
            var length = 0;
            while ((length = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fixed (byte* b = &(buffer[0]))
                    Uchardet.HandleData(b, (uint)length);
            }
            Uchardet.HandleDataEnd();
            StringBuilder sb = new StringBuilder();
            Uchardet.GetCharSet(sb, 4096);
            stream.Seek(streamPosition, SeekOrigin.Begin);
            return sb.ToString();
        }

        public static System.Text.Encoding Detect(Stream stream)
        {
            return System.Text.Encoding.GetEncoding(DetectImpl(stream));
        }

        public static System.Text.Encoding Detect(string filename)
        {
            using (var stream = new System.IO.FileStream(filename, System.IO.FileMode.Open))
            {
                return Detect(stream);
            }
        }
    }
}
