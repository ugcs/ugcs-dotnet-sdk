using AForge.Video;
using AutoMapper;
using Caliburn.Micro;
using MapControl;
using Services;
using Services.DTO;
using Services.Interfaces;
using Services.Log;
using Services.SdkServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using UGCS.Sdk.Protocol.Encoding;
using UGCS.Example.Models;
using UGCS.Example.Properties;
using Services.Commands;

namespace UGCS.Example.ViewModels
{
    public partial class MainViewModel : Caliburn.Micro.PropertyChangedBase
    {
        public Mission Mission { get; set; }
        public Route Route { get; set; }
        public ProcessedRoute ProcessedRoute { get; set; }
        public ClientVehicle ClientVehicle { get; set; }
        private MissionService _missionService;
        private RouteService _routeService;
        private VehicleService _vehicleService;
        private CommandService _commandService;
        private VehicleCommand _vehicleCommand;
        ILogger logger = new Logger(typeof(MainViewModel));
        public MapViewModel MapViewModel { get; set; }
        public MainViewModel() { }
        public MainViewModel(VehicleService vs, VehicleListener vl, MapViewModel mp, TelemetryListener tl, CommandService cs, MissionService ms, RouteService rs, VehicleCommand vc)
        {

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ServiceTelemetryDTO, ClientTelemetry>();
            });

            logger.LogInfoMessage("Main window initialized");
            MapViewModel = mp;
            _vehicleService = vs;
            _commandService = cs;
            _missionService = ms;
            _routeService = rs;
            _vehicleCommand = vc;
            try
            {
                ClientVehicle = new ClientVehicle();
                ClientVehicle.Vehicle = vs.GetVehicleByName(Settings.Default.UgcsDroneProfileName);
                ClientVehicle.Telemetry.Vehicle = ClientVehicle.Vehicle;
                var subscription = new ObjectModificationSubscription();
                subscription.ObjectId = ClientVehicle.Vehicle.Id;
                subscription.ObjectType = "Vehicle";
                _commandService.TryAcquireLock(ClientVehicle.Vehicle.Id);
                tl.AddVehicleIdTolistener(ClientVehicle.Vehicle.Id, TelemetryCallBack);
                vl.SubscribeVehicle(subscription, (e) =>
                {
                    //Subscribe vehicle changes
                });
                MapViewModel.Init(ClientVehicle);
                NotifyOfPropertyChange(() => MissionName);
                NotifyOfPropertyChange(() => RouteName);
                NotifyOfPropertyChange(() => VehicleName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Application.Current.Shutdown();
            }
        }

        public String RouteName
        {
            get
            {
                if (Route == null)
                {
                    return "Route not initialized";
                }
                return Route.Name;
            }
        }

        public String MissionName
        {
            get
            {
                if (Mission == null)
                {
                    return "Mission not initialized";
                }
                return Mission.Name;
            }
        }

        public String VehicleName
        {
            get
            {
                return ClientVehicle.Vehicle.Name;
            }
        }

        private String _centerMapView = "CenterMapView";
        public String CurrentCenterView
        {
            get
            {
                return _centerMapView;
            }
            set
            {
                _centerMapView = value;
                NotifyOfPropertyChange(() => CurrentCenterView);
            }
        }

        private String _currentMainView = "ExampleStartView";
        public String CurrentMainView
        {
            get
            {
                return _currentMainView;
            }
            set
            {
                _currentMainView = value;
                NotifyOfPropertyChange(() => CurrentMainView);
            }
        }

        public String WindowTitle
        {
            get
            {
                return "UCS Example .NET Client";
            }
        }

        private String _landCommandName = "Land";
        public String LandCommandName
        {
            get
            {
                return _landCommandName;
            }
            set
            {
                _landCommandName = value;
                NotifyOfPropertyChange(() => LandCommandName);
            }
        }

        bool? ledStatus = true;
        public bool? LedStatus
        {
            get
            {
                return ledStatus;
            }
            set
            {
                ledStatus = value;
                NotifyOfPropertyChange(() => LedStatus);
            }
        }
        private bool flash = true;
        public bool Flash
        {
            get
            {
                return flash;
            }
            set
            {
                flash = value;
                NotifyOfPropertyChange(() => Flash);
            }
        }

        private bool ledStatusSubsystems = false;
        public bool LedStatusSubsystems
        {
            get
            {
                return ledStatusSubsystems;
            }
            set
            {
                ledStatusSubsystems = value;
                NotifyOfPropertyChange(() => LedStatusSubsystems);
            }
        }

        private bool flashSubsystems = false;
        public bool FlashSubsystems
        {
            get
            {
                return flashSubsystems;
            }
            set
            {
                flashSubsystems = value;
                NotifyOfPropertyChange(() => FlashSubsystems);
            }
        }

        private bool _initMapCenter;
        public void TelemetryCallBack(ServiceTelemetryDTO dto, int tempId)
        {
            if (ClientVehicle != null)
            {
                Mapper.Map<ServiceTelemetryDTO, ClientTelemetry>(dto, ClientVehicle.Telemetry);
                if (ClientVehicle.Telemetry.LatitudeGPS != null && ClientVehicle.Telemetry.LongitudeGPS != null)
                {
                    if (!_initMapCenter)
                    {
                        _initMapCenter = true;
                        MapViewModel.InitMapCenter(ClientVehicle.Telemetry.LatitudeGPS.Value, ClientVehicle.Telemetry.LongitudeGPS.Value);
                    }
                    MapViewModel.ChangeCoordinates(ClientVehicle.Telemetry.LatitudeGPS.Value, ClientVehicle.Telemetry.LongitudeGPS.Value, ClientVehicle.Telemetry.AltitudeAGL, ClientVehicle.Telemetry.Camera1Yaw);
                }

            }
        }


        private Visibility _reconnectVideoVisible;
        public Visibility ReconnectVideoVisible
        {
            get
            {
                return _reconnectVideoVisible;
            }
            set
            {
                _reconnectVideoVisible = value;
                NotifyOfPropertyChange(() => ReconnectVideoVisible);
                NotifyOfPropertyChange(() => SliderControlVisible);
            }
        }

        public Visibility SliderControlVisible
        {
            get
            {
                if (ReconnectVideoVisible == Visibility.Visible)
                {
                    return Visibility.Hidden;
                }
                return Visibility.Visible;
            }
        }

        private String _videoSourceName;
        public String VideoSourceName
        {
            get
            {
                return _videoSourceName;
            }
            set
            {
                _videoSourceName = value;
            }
        }

        private String _currentCamera;
        public String CurrentCamera
        {
            get
            {
                return _currentCamera;
            }
            set
            {
                _currentCamera = value;
                NotifyOfPropertyChange(() => CurrentCamera);

            }
        }

        private String _videoMessage;
        public String VideoMessage
        {
            get
            {
                return _videoMessage;
            }
            set
            {
                _videoMessage = value;
                NotifyOfPropertyChange(() => VideoMessage);
            }
        }

        private BitmapImage _UCSvideoSource;
        public BitmapImage UCSVideoSource
        {
            get
            {
                return _UCSvideoSource;
            }
            set
            {
                _UCSvideoSource = value;
                NotifyOfPropertyChange(() => UCSVideoSource);
            }
        }

        private BitmapImage bitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                bitmapimage.Freeze();
                return bitmapimage;
            }
        }
    }
}
