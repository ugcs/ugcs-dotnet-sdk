using Ninject;
using Services.Interfaces;
using Services.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UGCS.Example.Enums;
using UGCS.Example.Properties;

namespace UGCS.Example
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IKernel Kernel { get; set; }
        ILogger logger = new Logger(typeof(App));
        public App()
        {
            logger.LogInfoMessage("App init");

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnmanagedException);

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Settings.Default.CurrentUICulture);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Settings.Default.CurrentUICulture);
            InitializeComponent();
            ShutdownMode = ShutdownMode.OnLastWindowClose;

        }

        private void UnmanagedException(object sender, UnhandledExceptionEventArgs args)
        {
            logger.LogError("!!!Unmanaged exception");
            Exception e = (Exception)args.ExceptionObject;
            logger.LogException(e);

            MessageBoxResult result = MessageBox.Show(e.Message + "\n" + args.IsTerminating + "\nApplication will exit", "Unmanaged exception", MessageBoxButton.OK, MessageBoxImage.Error);
            if (result == MessageBoxResult.OK)
            {
                Environment.Exit(0);
            }
        }
    }
}
