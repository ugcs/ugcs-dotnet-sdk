using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UgCS.SDK.Examples.Common;
using UGCS.Sdk.Protocol;
using UGCS.Sdk.Protocol.Encoding;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.Commands
{
    /// <summary>
    /// This class implements the logic that is common for all vehicle commands:
    /// - listen to UgCS server for changing state of the vehicle command and notify subscribers when state is changed.
    /// - If active vehicle is chaged, update unsubscribe from the old and subscribe to a new vehicle.
    /// </summary>
    internal abstract class VehicleCommandBase : ICommand, IDisposable
    {
        private UcsFacade _ucs;
        private SubscriptionToken _subscription;
        /// <summary>
        /// Null if we don't know is it available or no.
        /// </summary>
        private bool? _isEnabled = null;


        public event EventHandler CanExecuteChanged;


        /// <summary>
        /// Concrete command should return a command name.
        /// </summary>
        public abstract string Code { get; }

        public bool CanExecute(object parameter)
        {
            Vehicle activeVehicle = App.Current.ActiveVehicle;

            // return false if we don't know the actual availablity of the command
            return _isEnabled ?? false;
        }


        public VehicleCommandBase(UcsFacade ucs)
        {
            _ucs = ucs ?? throw new ArgumentNullException();

            Vehicle activeVehicle = App.Current.ActiveVehicle;
            if (activeVehicle != null)
            {
                _ucs.SubscribeToVehicleCommands(
                    activeVehicle,
                    onVehicleCommandStatesChanged,
                    out _subscription);
            }

            App.Current.ActiveVehicleChanged += app_ActiveVehicleChanged;
        }


        public void Execute(object parameter)
        {
            Vehicle activeVehicle = App.Current.ActiveVehicle;
            Debug.Assert(activeVehicle != null);

            // Before execute any command we must get exclusive access to the vehicle.
            // Basically we have to call this command once but to simplify the code
            // let's call it each time before upload.
            _ucs.AcquireLock(activeVehicle);

            ExecuteVehicleCommand(_ucs, activeVehicle, parameter);
        }

        protected abstract void ExecuteVehicleCommand(UcsFacade ucs, Vehicle v, object parameter);


        private void app_ActiveVehicleChanged(object sender, EventArgs e)
        {
            if (_subscription != null)
            {
                _ucs.Unsubscribe(_subscription);
                _subscription = null;
            }

            Vehicle activeVehicle = App.Current.ActiveVehicle;
            if (activeVehicle == null)
                return;

            _ucs.SubscribeToVehicleCommands(
                activeVehicle,
                onVehicleCommandStatesChanged,
                out _subscription);
        }


        private void setIsEnabled(bool? value)
        {
            if (value != _isEnabled)
            {
                _isEnabled = value;

                // In WPF we must to invoke command events from the UI thread
                App.Current.Dispatcher.Invoke(
                    () => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
            }
        }

        private void onVehicleCommandStatesChanged(CommandEvent e)
        {
            Debug.Assert(e.Commands != null);

            CommandDefinition cmd = e.Commands.FirstOrDefault(
                c => String.Equals(c.Code, Code, StringComparison.InvariantCultureIgnoreCase)
                    && c.Subsystem == Subsystem.S_FLIGHT_CONTROLLER);

            if (cmd == null || !cmd.EnabledSpecified)
                setIsEnabled(null);
            else
                setIsEnabled(cmd.Enabled);
        }

        public void Dispose()
        {
            if (_subscription == null)
                return;

            try
            {
                _ucs.Unsubscribe(_subscription);
            }
            catch (Exception err)
            {
                // Errors mustn't be raised in Dispose
                Trace.TraceError(err.ToString());
            }
        }
    }
}
