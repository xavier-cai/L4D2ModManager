using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L4D2ModManager
{
    class SteamDelegate : IDisposable
    {
        private static string Program = "delegate.exe";
        private static string CacheFolder = "cache\\";

        private class JsonResult
        {
            public bool Result { get; set; }
        }

        public class DelegateResult
        {
            public string Json { get; private set; }
            public string Description { get; private set; }
            public ulong FileId { get; private set; }
            public DelegateResult(string json, string des, ulong id)
            {
                Json = json;
                Description = des;
                FileId = id;
            }
        }

        public List<ulong> FileId { get; set; }
        public bool Timeout { get; private set; }
        public bool Result { get; private set; }
        public bool IsRunning { get; private set; }

        private System.Diagnostics.Process Process;

        public SteamDelegate()
        {
            Timeout = false;
            Result = false;
            IsRunning = false;
        }

        public SteamDelegate(List<ulong> ids)
            : this()
        {
            FileId = ids;
        }

        public void RunDelegate()
        {
            Logging.Assert(!IsRunning);
            Timeout = false;
            Result = false;
            if (!System.IO.File.Exists(Program))
                return;
            StringBuilder sb = new StringBuilder();
            foreach (var v in FileId)
                sb.Append(' ' + v.ToString());
            if (!System.IO.Directory.Exists(CacheFolder))
                System.IO.Directory.CreateDirectory(CacheFolder);
            Process = new System.Diagnostics.Process();
            Process.StartInfo.CreateNoWindow = true;
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.RedirectStandardInput = true;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.StartInfo.RedirectStandardError = true;
            Process.StartInfo.FileName = Program;
            Process.StartInfo.Arguments = sb.ToString();
            Process.Start();

            Timeout = true;
            Action timeoutAction = () =>
            {
                string verify = Process.StandardOutput.ReadLine();
                Logging.Assert(verify.Equals("1"));
                Timeout = false;
            };
            var timeoutThread = new System.Threading.Thread(new System.Threading.ThreadStart(timeoutAction));
            timeoutThread.Start();
            timeoutThread.Join(5000);
            if (Timeout)
                return;
            string jsonResult = Process.StandardOutput.ReadLine();
            Result = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonResult>(jsonResult).Result;
            if (!Result)
                return;
        }

        public DelegateResult Read()
        {
            ulong key = Convert.ToUInt64(Process.StandardOutput.ReadLine());
            if (key == 0)
                return null;
            try
            {
                DelegateResult ret = null;
                int readCount = 5;
                while(ret == null && readCount-- > 0)
                {
                    try
                    {
                        var result = new DelegateResult(System.IO.File.ReadAllText(CacheFolder + key.ToString() + ".json"),
                        System.IO.File.ReadAllText(CacheFolder + key.ToString() + ".des"), key);
                        ret = result;
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(10); //can not read the file, have a rest
                    }
                }
                if (ret == null)
                    throw new TimeoutException();
                return ret;
            }
            catch { }
            return new DelegateResult("", "", 0); //read failed
        }

        public void Close()
        {
            if(IsRunning)
            {
                IsRunning = false;
                Process.Kill();
                Process.Close();
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
