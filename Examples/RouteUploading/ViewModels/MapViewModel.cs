using CalculatedProperties;
using MapControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using UgCS.SDK.Examples.Common;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;
using static UgCS.SDK.Examples.Common.TelemetryUtils;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.ViewModels
{
    public class PointItem
    {
        public string Name { get; set; }
        public Location Location { get; set; }
    }

    public class MapViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly PropertyHelper _property;


        public event PropertyChangedEventHandler PropertyChanged;


        public IMapLayer Layer { get; set; }

        public ObservableCollection<PointItem> Points { get; } = new ObservableCollection<PointItem>();

        public ObservableCollection<LocationCollection> Lines { get; } = new ObservableCollection<LocationCollection>();

        public int? SelectedWaypointIndex
        {
            get => _property.Get((int?)null);
            set => _property.Set(value);
        }

        public ActiveVehicleViewModel ActiveVehicle { get; }
        

        public string UploadDetails
        {
            get
            {
                return _property.Calculated(() =>
                {
                    if (SelectedWaypointIndex == null)
                        return "from the begining";
                    else
                        return "from waypoint #" + SelectedWaypointNumber;

                });
            }
        }

        public int? SelectedWaypointNumber
        {
            get
            {
                return _property.Calculated(() => SelectedWaypointIndex + 1);
            }
        }


        public MapViewModel(UcsFacade ucs)
        {
            _property = new PropertyHelper(onPropertyChanged);
            Layer = new WmsImageLayer
            {
                ServiceUri = new Uri("http://ows.terrestris.de/osm/service"),
                Layers = "OSM-WMS"
            };

            App.Current.ActiveRouteChanged += app_ActiveRouteChanged;

            ActiveVehicle = new ActiveVehicleViewModel(ucs);
        }

        private void app_ActiveRouteChanged(object sender, EventArgs e)
        {
            Lines.Clear();
            Points.Clear();
            SelectedWaypointIndex = null;


            ProcessedRoute processedRoute = App.Current.ActiveRoute;
            IList<Waypoint> wps = processedRoute.GetWaypoints();

            var routePath = new LocationCollection();
            for (int i = 0; i < wps.Count; i++)
            {
                var location = new Location(
                        wps[i].Latitude.ToDegrees(),
                        wps[i].Longitude.ToDegrees());
                Points.Add(new PointItem
                {
                    Name = (i + 1).ToString(),
                    Location = location
                });
                routePath.Add(location);
            }

            Lines.Add(routePath);
        }


        private void onPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
        }

        public void Dispose()
        {
            App.Current.ActiveRouteChanged -= app_ActiveRouteChanged;
            ActiveVehicle.Dispose();
        }
    }
}
