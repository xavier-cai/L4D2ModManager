using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace L4D2ModManager
{
    /// <summary>
    /// App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
#if DEBUG
#else
            this.Startup += new StartupEventHandler(OnStartup);
            this.Exit += new ExitEventHandler(OnExit);
#endif
        }

        public void OnStartup(object sender, StartupEventArgs e)
        {
            Action<string, object> OnException = (str, exp) =>
            {
                try
                {
                    var err = exp as Exception;
                    Logging.Error(str + "\r\nSource:" + err.Source + "\r\nMessage:" + err.Message + "\r\nTrace Stack:\r\n" + err.StackTrace);
                }
                catch
                {
                    Logging.Error("Unknown exception : " + exp.ToString());
                }
            };

            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler((o, err) =>
            {
                OnException("UI thread ERROR -> DispatcherUnhandledException", err.Exception);
            });
            TaskScheduler.UnobservedTaskException += (o, err) =>
            {
                OnException("Task thread ERROR -> UnobservedTaskException", err.Exception);
            };
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler((o, err) =>
            {
                OnException("non-UI thread ERROR -> UnhandledException", err.ExceptionObject);
            });
        }

        public void OnExit(object sender, ExitEventArgs e)
        {

        }
    }
}
