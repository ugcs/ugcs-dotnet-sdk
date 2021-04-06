using CalculatedProperties;
using MapControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UgCS.SDK.Examples.Common;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;
using static UgCS.SDK.Examples.Common.TelemetryUtils;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.ViewModels
{
    public sealed class ActiveVehicleViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly PropertyHelper _property;
        private UcsFacade _ucs;
        private SubscriptionToken _telemetrySubscription;


        public event PropertyChangedEventHandler PropertyChanged;


        public double? Heading
        {
            get => _property.Get((double?)null);
            set => _property.Set(value);
        }

        public double? Latitude
        {
            get => _property.Get((double?)null);
            set => _property.Set(value);
        }

        public double? Longitude
        {
            get => _property.Get((double?)null);
            set => _property.Set(value);
        }

        public double? AltitudeRaw
        {
            get => _property.Get((double?)null);
            set => _property.Set(value);
        }


        public Location Location
        {
            get
            {
                return _property.Calculated(() =>
                {
                    if (Latitude == null || Longitude == null)
                        return null;
                    return new Location(Latitude.Value, Longitude.Value);
                });
            }
        }

        public bool IsDisplayed
        {
            get
            {
                return _property.Calculated(() => Location != null && Heading != null);
            }
        }


        public ActiveVehicleViewModel(UcsFacade ucs)
        {
            _ucs = ucs ?? throw new ArgumentNullException();
            _property = new PropertyHelper(onPropertyChanged);

            Vehicle v = App.Current.ActiveVehicle;
            if (v != null)
                ucs.SubscribeToVehicleTelemetry(v, onTelemetryReceived, out SubscriptionToken _telemetrySubscription);
            App.Current.ActiveVehicleChanged += app_ActiveVehicleChanged;

        }


        private void onPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
        }

        private void app_ActiveVehicleChanged(object sender, EventArgs e)
        {
            if (_telemetrySubscription != null)
            {
                _ucs.Unsubscribe(_telemetrySubscription);
                _telemetrySubscription = null;
            }

            Vehicle v = App.Current.ActiveVehicle;
            if (v != null)
                _ucs.SubscribeToVehicleTelemetry(v, onTelemetryReceived, out _telemetrySubscription);
        }

        private void onTelemetryReceived(TelemetryEvent e)
        {
            Debug.Assert(e.Telemetry != null);

            // Since we use CalculatedProperties lib we must access properties from the UI thread
            App.Current.Dispatcher.Invoke(() =>
            {
                if (e.Telemetry.Contains(WellKnownTelemetry.Latitude, out TlmValue<double> lat))
                {
                    if (lat.IsUnknown)
                        Latitude = null;
                    else
                        Latitude = lat.Value.ToDegrees();
                }

                if (e.Telemetry.Contains(WellKnownTelemetry.Longitude, out TlmValue<double> lon))
                {
                    if (lon.IsUnknown)
                        Longitude = null;
                    else
                        Longitude = lon.Value.ToDegrees();
                }

                if (e.Telemetry.Contains(WellKnownTelemetry.Heading, out TlmValue<double> heading))
                {
                    if (heading.IsUnknown)
                        Heading = null;
                    else
                        Heading = heading.Value.ToDegrees();
                }

                if (e.Telemetry.Contains(WellKnownTelemetry.AltitudeRaw, out TlmValue<double> alt))
                {
                    AltitudeRaw = alt;
                }
            });
        }

        public void Dispose()
        {
            if (_telemetrySubscription != null)
            {
                App.Current.ActiveVehicleChanged -= app_ActiveVehicleChanged;
                try
                {
                    _ucs.Unsubscribe(_telemetrySubscription);
                }
                catch (Exception err)
                {
                    // Dispose musn't raise errors
                    Trace.TraceError(err.ToString());
                }
            }
        }
    }
}
