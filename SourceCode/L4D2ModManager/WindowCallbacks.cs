using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L4D2ModManager
{
    static class WindowCallbacks
    {
        public delegate void ProcessCallback(int index, int total, string info);
        public delegate void PrintCallback(string info);
        public delegate void OperationEnableCallback(string key, bool enable);
        public delegate void NotifyUpdateCallback(string key, object obj);

        static private ProcessCallback _ProcessCallback;
        static private PrintCallback _PrintCallback;
        static private OperationEnableCallback _OperationEnableCallback;
        static private NotifyUpdateCallback _NotifyUpdateCallback;

        static public void SetProcessCallback(ProcessCallback cb)
        {
            _ProcessCallback = cb;
        }

        static public void Process(int index, int total, string info)
        {
            if (_ProcessCallback != null)
                _ProcessCallback.Invoke(index, total, info);
        }

        static public void SetPrintCallback(PrintCallback cb)
        {
            _PrintCallback = cb;
        }

        static public void Print(string info)
        {
            if (_PrintCallback != null)
                _PrintCallback.Invoke(info);
        }

        static public void SetOperationEnableCallback(OperationEnableCallback cb)
        {
            _OperationEnableCallback = cb;
        }

        static public void OperationEnable(string key, bool enable)
        {
            if (_OperationEnableCallback != null)
                _OperationEnableCallback(key, enable);
        }

        static public void SetNotifyUpdateCallback(NotifyUpdateCallback cb)
        {
            _NotifyUpdateCallback = cb;
        }

        static public void NotifyRefresh(string key, object obj)
        {
            if (_NotifyUpdateCallback != null)
                _NotifyUpdateCallback(key, obj);
        }
    }
}
