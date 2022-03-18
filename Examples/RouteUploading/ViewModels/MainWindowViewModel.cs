using CalculatedProperties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UgCS.SDK.Examples.Common;
using UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.Commands;
using UGCS.Sdk.Protocol.Encoding;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.ViewModels
{
    internal sealed class MainWindowViewModel: INotifyPropertyChanged, IDisposable
    {
        private readonly PropertyHelper _property;

        public event PropertyChangedEventHandler PropertyChanged;

        
        public ICommand OpenRoute { get; }

        public ICommand Upload { get; }

        public ICommand Arm { get; }

        public ICommand Auto { get; }

        public ICommand Manual { get; }

        public MapViewModel Map { get; }

        public ObservableCollection<Vehicle> Vehicles { get; } = new ObservableCollection<Vehicle>();

        public Vehicle SelectedVehicle
        {
            get  => _property.Get((Vehicle)null);
            set => _property.Set(value);
        }

        public MainWindowViewModel(UcsFacade ucs)
        {
            OpenRoute = new OpenRouteCommand(ucs)
                .AddErrorHandler(command_onError);
            Upload = new UploadCommand(ucs)
                .AddErrorHandler(command_onError);
            Arm = new ArmCommand(ucs)
                .AddErrorHandler(command_onError);
            Auto = new AutoCommand(ucs)
                .AddErrorHandler(command_onError);
            Manual = new ManualCommand(ucs)
                .AddErrorHandler(command_onError);


            Map = new MapViewModel(ucs);

            ucs.List<Vehicle>()
                .ForEach(x => Vehicles.Add(x));

            _property = new PropertyHelper(onPropertyChanged);
        }


        private void command_onError(ICommand source, Exception error, out bool wasErrorHandled)
        {
            MessageBox.Show(
                concatinateMessagesFromInnerExceptions(error), 
                "Command execution failed", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
            wasErrorHandled = true;
        }

        private string concatinateMessagesFromInnerExceptions(Exception error)
        {
            var totalMessage = new StringBuilder(error.Message);

            Exception e = error;
            while ((e = e.InnerException) != null)
            {
                totalMessage.AppendLine(".");
                totalMessage.Append(e.Message);
            }

            return totalMessage.ToString();
        }

        private void onPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);

            if (e.PropertyName == nameof(SelectedVehicle))
                App.Current.ActiveVehicle = SelectedVehicle;
        }

        public void Dispose()
        {
            Map.Dispose();
        }
    }
}
