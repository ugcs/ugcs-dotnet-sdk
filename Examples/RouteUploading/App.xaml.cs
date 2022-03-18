using CalculatedProperties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using UGCS.Sdk.Protocol.Encoding;

[assembly: DebuggerDisplay("[Field={TelemetryField}; Value={Value}]", Target = typeof(Telemetry))]

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged
    {
        private readonly PropertyHelper _property;


        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ActiveRouteChanged;
        public event EventHandler ActiveVehicleChanged;


        public static new App Current => (App)Application.Current;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        public ProcessedRoute ActiveRoute
        {
            get => _property.Get((ProcessedRoute)null);
            set => _property.Set(value);
        }

        public Vehicle ActiveVehicle
        {
            get => _property.Get((Vehicle)null);
            set => _property.Set(value);
        }


        public App()
        {
            _property = new PropertyHelper(onPropertyChanged);
        }


        private void onPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));

            switch (e.PropertyName)
            {
                case nameof(ActiveRoute): ActiveRouteChanged?.Invoke(this, EventArgs.Empty); break;
                case nameof(ActiveVehicle): ActiveVehicleChanged?.Invoke(this, EventArgs.Empty); break;
            }
        }
    }
}
