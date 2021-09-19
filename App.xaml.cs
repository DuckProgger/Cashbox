using Cashbox.Visu;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Cashbox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private static Logger _logger = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetupExceptionHandling();
        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                MessageBoxCustom.Show("Возникло необработанное исключение" + e.ExceptionObject, MessageType.Error, MessageButtons.Ok);
                //LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");
            };

            DispatcherUnhandledException += (s, e) =>
            {
                MessageBoxCustom.Show("Возникло необработанное исключение" + e.Exception, MessageType.Error, MessageButtons.Ok);
                //LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            //Dispatcher.UnhandledException += (s, e) =>
            //{
            //    MessageBoxCustom.Show("Возникло необработанное исключение" + e.Exception, MessageType.Error, MessageButtons.Ok);
            //    //LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
            //    e.Handled = true;
            //};

            TaskScheduler.UnobservedTaskException += (s, e) =>
           {
               MessageBoxCustom.Show("Возникло необработанное исключение" + e.Exception, MessageType.Error, MessageButtons.Ok);
                //LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
           };

        }

        //private void LogUnhandledException(Exception exception, string source)
        //{
        //    string message = $"Unhandled exception ({source})";
        //    try
        //    {
        //        AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
        //        message = $"Unhandled exception in {assemblyName.Name} v{assemblyName.Version}";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex, "Exception in LogUnhandledException");
        //    }
        //    finally
        //    {
        //        _logger.Error(exception, message);
        //    }
        //}
    }
}
