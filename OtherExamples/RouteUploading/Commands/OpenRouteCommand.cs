using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UgCS.SDK.Examples.Common;
using UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.Views;
using UGCS.Sdk.Protocol.Encoding;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.Commands
{
    internal sealed class OpenRouteCommand : ICommand
    {
        private readonly UcsFacade _ucs;


        public OpenRouteCommand(UcsFacade ucs)
        {
            _ucs = ucs ?? throw new ArgumentNullException();
        }

#pragma warning disable CS0067 // CanExecute returns constant
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (!SelectRouteDialog.Show(_ucs, out int selectedRouteId))
                return;

            ProcessedRoute processedRoute;
            try
            {
                var selectedRoute = _ucs.Get<Route>(selectedRouteId);
                processedRoute = _ucs.Process(selectedRoute);
            }
            catch (Exception err)
            {
                MessageBox.Show(
                    $"An error occured during route processing: {err.Message}",
                    $"Can't open route #{selectedRouteId}.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (processedRoute.HasErrors())
            {
                MessageBox.Show(
                    $"The route contains errors and couldn't be processed. You can see details in UgCS.",
                    $"Can't open route #{selectedRouteId}.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            App.Current.ActiveRoute = processedRoute;
        }
    }
}
