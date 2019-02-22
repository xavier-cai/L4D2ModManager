using System;
using System.Runtime.InteropServices;

namespace L4D2ModManager
{

    class Logging
    {
        [DllImport("Kernel32.dll")]
        private static extern bool AllocConsole(); 
        [DllImport("kernel32.dll", EntryPoint = "FreeConsole")]
        private static extern bool FreeConsole(); 
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        extern static IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags); 
        [DllImport("Kernel32.dll")]
        public static extern bool SetConsoleTitle(string strMessage);  


        private Logging(bool create)
        {
#if DEBUG
            if (create)
            {
                AllocConsole();
                IntPtr windowHandle = FindWindow(null, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
                uint SCCLOSE = 0xF060;
                RemoveMenu(closeMenu, SCCLOSE, 0x0);
                SetConsoleTitle("Debug Console");
            }
#endif
        }

        ~Logging()
        {
#if DEBUG
            FreeConsole();
#endif
        }

        static private Logging Instance = new Logging(false);
        static private void WriteLine(string msg)
        {
#if DEBUG
            Console.WriteLine(msg);
#endif
        }

#if DEBUG
        static public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }
#endif

        static public void Pause()
        {
#if DEBUG
            ReadKey();
#endif
        }

        static string LogFile = "log.txt";
        static bool LogFileInitialized = false;
        static object LockObject = new object();
        static public void Log<T>(T msg, string level = "Normal")
        {
#if DEBUG
            WriteLine('[' + level + ']' + ' ' + msg.ToString());
#else
            lock (LockObject)
            {
                if (Configure.ReleaseLogToFile)
                {
                    if (!LogFileInitialized)
                    {
                        System.IO.File.WriteAllText(LogFile, DateTime.Now.ToString() + "\r\n");
                        LogFileInitialized = true;
                    }
                    System.IO.File.AppendAllText(LogFile, '[' + level + ']' + ' ' + msg.ToString() + "\r\n");
                }
            }
#endif
        }

        static public void Warn<T>(T msg)
        {
            Log(msg.ToString(), "Warn");
        }

        static public void Error(string msg = "", string level = "Error")
        {
            Log(msg, level);
            Pause();
            //MessageBox.Show(msg, level);
            System.Environment.Exit(0);
        }

        static public void Assert(bool cond, string msg = "")
        {
            if (!cond)
            {
                throw new InvalidOperationException();
                Error(msg, "ASSERT");
            }
        }
    }
}
