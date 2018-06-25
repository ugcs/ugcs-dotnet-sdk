using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
#if NETFX_CORE
using Windows.UI.Xaml;
#else
using System.Windows.Threading;
#endif
using MapControl;
using UGCS.Example.Models;
using Services;
using System.Windows;
using UGCS.Example.Properties;
using System.Windows.Media;
using UGCS.Sdk.Protocol.Encoding;

namespace UGCS.Example.ViewModels
{
    public class MapViewModel : Caliburn.Micro.PropertyChangedBase
    {
        private bool firstInit = false;
        public Brush LineColors
        {
            get
            {
                return MapResource.DrawLinesColor;
            }
        }
        public double CircleWidth
        {
            get
            {
                return 300;
            }
        }

        public ObservableCollection<Polyline> Polylines { get; set; }

        public double CircleWidthNegative
        {
            get
            {
                return CircleWidth * (-1);
            }
        }

        public Size ArcSegmentSize
        {
            get
            {
                return new Size(CircleWidth, CircleWidth);
            }
        }

        public String StringHeading
        {
            get
            {
                var cam = _cameraFOVH;
                if (cam < 0)
                {
                    cam = cam + 360;
                }
                return "Vehicle heading: " + Math.Round(cam);
            }
        }

        public ArcSector ArcSector { get; set; }

        public ObservableCollection<MapPoint> Points { get; set; }

        private Location mapCenter;
        public Location MapCenter
        {
            get
            {
                return mapCenter;
            }
            set
            {
                mapCenter = value;
                NotifyOfPropertyChange(() => MapCenter);
            }
        }

        private Double _zoom = 17.5;
        public Double Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
                NotifyOfPropertyChange(() => MapScale);
                NotifyOfPropertyChange(() => CircleWidth);
                NotifyOfPropertyChange(() => Zoom);
            }
        }

        private ClientTileLayer _clientTileLayer = new ClientTileLayer();
        public TileLayer MapResource
        {
            get
            {
                return _clientTileLayer.TileLayers.FirstOrDefault(x => x.SourceName == Settings.Default.MapProviderName);
            }
        }

        public String MapScale
        {
            get
            {
                if (WPFMap == null)
                {
                    return "0 m";
                }
                var mapSacale = WPFMap.GetMapScale(MapCenter);
                var ms = CircleWidth / mapSacale;
                if (Math.Round(ms) <= 1000)
                {
                    ms = Math.Round(ms, 0);
                    return ms.ToString("0 m");
                }
                else
                {
                    ms = Math.Round(ms / 1000, 2);
                    return ms.ToString("0.00 m");
                }
            }
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private Double _cameraFOVH;
        public void ChangeCoordinates(float latitudeGPS, float longitudeGPS, float? altitude, float? cameraFOVH)
        {
            if (Points == null)
            {
                return;
            }
            if (cameraFOVH != null)
            {
                _cameraFOVH = RadianToDegree(cameraFOVH.Value);
                NotifyOfPropertyChange(() => StringHeading);
            }
            var latDeg = RadianToDegree(latitudeGPS);
            var lonDeg = RadianToDegree(longitudeGPS);
            var point = Points.FirstOrDefault().Location;
            if (point != null && latDeg == point.Latitude && lonDeg == point.Longitude)
            {
                return;
            }
            Points.FirstOrDefault().Location = new Location(RadianToDegree(latitudeGPS), RadianToDegree(longitudeGPS));
            if (!firstInit)
            {
                Zoom = 17.5;
                firstInit = true;
            }

        }
        public void InitMapCenter(float latitudeGPS, float longitudeGPS)
        {
            //init map center
            //Example: MapCenter = new Location(RadianToDegree(latitudeGPS), RadianToDegree(longitudeGPS));
            MapCenter = new Location(RadianToDegree(0.8163854), RadianToDegree(0.1456502));
            
        }

        public void UpdateRouteView(Route route)
        {
            var locationCollection = new LocationCollection();
            foreach (var segment in route.Segments)
            {
                var point = segment.Figure.Points.First();
                locationCollection.Add(new Location()
                    {
                        Latitude = RadianToDegree(point.Latitude),
                        Longitude = RadianToDegree(point.Longitude)
                    }
                );
            }
            Polylines.FirstOrDefault().Locations = locationCollection;
        
        }

        public Map WPFMap { get; set; }

        public MapViewModel()
        {
            Polylines = new ObservableCollection<Polyline>(); 
            Polylines.Add(
                new Polyline
                {
                    Name = "ExampleRoute",
                    Locations = new LocationCollection()
                });
        }

        public void Init(ClientVehicle cv)
        {

            //MapCenter = new Location(RadianToDegree(cv.Telemetry.LatitudeGPS.Value), RadianToDegree(cv.Telemetry.LongitudeGPS.Value));
            InitMapCenter(0, 0);
            if (cv.Telemetry.LatitudeGPS == null || cv.Telemetry.LongitudeGPS == null)
            {
                Zoom = 7;
            }

            Points = new ObservableCollection<MapPoint>();
            if (cv.Telemetry.LatitudeGPS != null && cv.Telemetry.LongitudeGPS != null)
            {

                Points.Add(
                    new MapPoint
                    {
                        Name = cv.Vehicle.Name,
                        Location = new Location(RadianToDegree(cv.Telemetry.LatitudeGPS.Value), RadianToDegree(cv.Telemetry.LongitudeGPS.Value))
                    });
            }
            else
            {
                Points.Add(
                    new MapPoint
                    {
                        Name = cv.Vehicle.Name,
                        Location = null
                    });
            }
            ArcSector = new ArcSector();
            ArcSector.SetAngle(0, 0, CircleWidth);           

        }
    }
}
