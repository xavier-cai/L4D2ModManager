using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace L4D2ModManager
{
    class WebHelper
    {
        static public string GetWebClient(string url)
        {
            WebClient myWebClient = new WebClient();
            Stream myStream = myWebClient.OpenRead(url);
            StreamReader sr = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
            string strHTML = sr.ReadToEnd();
            myStream.Close();
            return strHTML;
        }
    }
}
