using AForge.Video;
using Caliburn.Micro;
using Ninject;
using Services;
using Services.Exceptions;
using Services.Helpers;
using Services.Interfaces;
using Services.Log;
using Services.SdkServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;
using UGCS.Sdk.Tasks;
using UGCS.Example.Properties;
using UGCS.Example.ViewModels;
using Services.Commands;

namespace UGCS.Example
{
    public class AppBootstrapper : BootstrapperBase
    {
        private Boolean _isInitialized;
        private IConnect _connection;
        private TcpClient _tcpClient;
        private MessageSender _messageSender;
        private MessageReceiver _messageReceiver;
        private MessageExecutor _messageExecutor;
        private NotificationListener _notificationListener;
         
        private SplashScreen splashScreen = new SplashScreen("Resources/SplashScreen.png");
        ILogger logger = new Logger(typeof(AppBootstrapper));

        public IKernel Kernel
        {
            get
            {
                return App.Kernel;
            }
            set
            {
                App.Kernel = value;
            }
        }
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            splashScreen.Show(false);
            try
            {
            
                Kernel = new StandardKernel();
                Settings.Default.IsValidConfiguration();
                _tcpClient = new TcpClient();
                _tcpClient.Connect(Settings.Default.UgcsUcsAddress, Settings.Default.UCSPort);
                _tcpClient.Session.Disconnected += UCSDisconnect;
                _messageSender = new MessageSender(_tcpClient.Session);
                _messageReceiver = new MessageReceiver(_tcpClient.Session);                
                _messageExecutor = new MessageExecutor(_messageSender, _messageReceiver, new InstantTaskScheduler());
                _messageExecutor.Configuration.DefaultTimeout = 10 * 1000;
                _messageExecutor.Configuration.SetTimeout(typeof(ProcessRouteRequest), 10 * 1000);
                _messageExecutor.Configuration.SetTimeout(typeof(CreateOrUpdateObjectRequest), 10 * 1000);
                int timeout = 10 * 1000;
                Type[] transferTypes = new[] {
							typeof(ExportRouteToXmlRequest),
							typeof(ImportRouteRequest),
							typeof(ExportMissionToXmlRequest),
							typeof(ImportMissionFromXmlRequest),
							typeof(ExportTelemetryRequest),
							typeof(ImportTelemetryRequest)
						};
                foreach (Type type in transferTypes)
                {
                    _messageExecutor.Configuration.SetTimeout(type, timeout);
                }
                
                _notificationListener = new NotificationListener();
                _messageReceiver.AddListener(-1, _notificationListener);


                Kernel.Bind<TcpClient>().ToConstant(_tcpClient);
                
                Kernel.Bind<LoginRequest>().ToConstant(new LoginRequest());
                Kernel.Bind<MessageExecutor>().ToConstant(_messageExecutor);
                Kernel.Bind<AuthorizeHciRequest>().ToConstant(new AuthorizeHciRequest()
                {
                    Locale = "en-US"
                });
                Kernel.Bind<TelemetrySubscription>().ToConstant(new TelemetrySubscription());
                Kernel.Bind<ObjectModificationSubscription>().ToConstant(new ObjectModificationSubscription());
                Kernel.Bind<EventSubscriptionWrapper>().ToConstant(new EventSubscriptionWrapper());
                Kernel.Bind<Mission>().ToConstant(new Mission());
                Kernel.Bind<NotificationListener>().ToConstant(_notificationListener);
                Kernel.Bind<Command>().ToConstant(new Command());
                Kernel.Bind<IConnect>().To<Connect>().InSingletonScope();
                Kernel.Bind<VehicleService>().ToSelf();
                Kernel.Bind<MissionService>().ToSelf();
                Kernel.Bind<RouteService>().ToSelf();
                Kernel.Bind<CommandService>().ToSelf();
                Kernel.Bind<MJPEGStream>().ToSelf();
                Kernel.Bind<TelemetryListener>().ToSelf().InSingletonScope();
                Kernel.Bind<MissionListener>().ToSelf().InSingletonScope();
                Kernel.Bind<VehicleListener>().ToSelf().InSingletonScope();
                Kernel.Bind<ServerLogListener>().ToSelf().InSingletonScope();
                

                Kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
                Kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
                Kernel.Bind<MainViewModel>().ToSelf().InSingletonScope();
                Kernel.Bind<MapViewModel>().ToSelf().InSingletonScope();
                Kernel.Bind<VehicleCommand>().ToSelf();
                Kernel.Bind<MappingRequestService>().ToSelf();

                _connection = Kernel.Get<IConnect>();
                var telemetryListener = Kernel.Get<TelemetryListener>();
                _connection.ConnectUgcs(Settings.Default.UCSLogin, Settings.Default.UCSPassword, () =>
                {
                    telemetryListener.SubscribeTelemtry();

                }).Wait();
                _isInitialized = true;

            }
            catch (Exception ex)
            {
                splashScreen.Close(new TimeSpan(0));
                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(ConnectionException))
                {
                    logger.LogException(ex.InnerException);
                    MessageBox.Show(ex.InnerException.Message);
                }
                else
                {
                    logger.LogException(ex);
                    MessageBox.Show(ex.Message);
                }
                Environment.Exit(0);
            }
        }


        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            if (_isInitialized)
            {
                DisplayRootViewFor<MainViewModel>();
            }
            splashScreen.Close(new TimeSpan(0));
        }
        protected override void OnExit(object sender, EventArgs e)
        {
            closeConnection();
            logger.LogInfoMessage("Application exit");
            Kernel.Dispose();
            base.OnExit(sender, e);
            Environment.Exit(0);
        }

        protected override object GetInstance(Type service, string key)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return Kernel.Get(service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            Kernel.Inject(instance);
        }

        private void closeConnection()
        {
            if (_messageReceiver != null)
            {
                _messageReceiver.Cancel();
                _messageReceiver.RemoveListener(-1);
            }
            if (_messageSender != null)
            {
                _messageSender.Cancel();
            }
            if (_tcpClient != null && _tcpClient.Session != null)
            {
                _tcpClient.Session.Disconnected -= UCSDisconnect;
            }
            if (_notificationListener != null)
            {
                _notificationListener.Dispose();
            }
            if (_tcpClient != null)
            {
                _tcpClient.Dispose();
            }
        }

        private void UCSDisconnect(object sender, EventArgs e)
        {
            closeConnection();
            logger.LogWarningMessage("UCS disconnected");
        }
    }
}
