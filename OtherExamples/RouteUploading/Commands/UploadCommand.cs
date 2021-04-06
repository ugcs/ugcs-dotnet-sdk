using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UgCS.SDK.Examples.Common;
using UGCS.Sdk.Protocol.Encoding;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.Commands
{
    internal sealed class UploadCommand : ICommand, IDisposable
    {
        private UcsFacade _ucs;


        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return App.Current.ActiveRoute != null && App.Current.ActiveVehicle != null;
        }

        public void Execute(object parameter)
        {
            int startIndex = 0;
            if (parameter != null && parameter is int)
                startIndex = (int)parameter;

            ProcessedRoute route = App.Current.ActiveRoute;
            Vehicle vehicle = App.Current.ActiveVehicle;

            Debug.Assert(route != null);
            Debug.Assert(vehicle != null);

            _ucs.AcquireLock(vehicle);
            _ucs.Upload(vehicle, route, startIndex);
        }


        public UploadCommand(UcsFacade ucs)
        {
            _ucs = ucs ?? throw new ArgumentNullException();
            App.Current.PropertyChanged += app_PropertyChanged;
        }

        private void app_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(App.Current.ActiveVehicle):
                case nameof(App.Current.ActiveRoute):
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    break;
            }
        }

        private void app_ActiveRouteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            App.Current.PropertyChanged -= app_PropertyChanged;
        }
    }
}
