using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using UgCS.SDK.Examples.Common;
using UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.ViewModels;
using UGCS.Sdk.Protocol.Encoding;
using static UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.ViewModels.OpenRouteDialogViewModel;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.Views
{
    /// <summary>
    /// Interaction logic for OpenRouteDialog.xaml
    /// </summary>
    public partial class SelectRouteDialog : Window
    {
        UcsFacade _ucs;
        private int? _selectedRouteId = null;
        

        private SelectRouteDialog(UcsFacade ucs)
        {
            InitializeComponent();

            _ucs = ucs ?? throw new ArgumentNullException(nameof(ucs));
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = new OpenRouteDialogViewModel();
            DataContext = vm;

            var getRouteReq = new GetObjectListRequest
            {
                ClientId = _ucs.ClientId,
                ObjectType = "Route",
                RefreshDependencies = true
            };

            var response = await _ucs.ExecuteAsync<GetObjectListResponse>(getRouteReq);
            response.Objects
                .Select(x => x.Route)
                .OrderBy(x => x.Mission.Name)
                .ThenBy(x => x.Name)
                .Select(x => new RouteViewModel(x.Id, getFullName(x)))
                .ToList()
                .ForEach(x => vm.Routes.Add(x));
            
        }

        /// <summary>
        /// If a route was selected, returns the route id.
        /// Otherwise null.
        /// </returns>
        public static bool Show(UcsFacade ucs, out int routeId)
        {
            var dlg = new SelectRouteDialog(ucs);

            dlg.ShowDialog();

            int? selectedId = dlg._selectedRouteId;
            if (selectedId == null)
            {
                routeId = 0;
                return false;
            }

            routeId = selectedId.Value;
            return true ;
        }

        private string getFullName(Route r)
        {
            return String.Format("{0}\\{1}", r.Mission.Name, r.Name);
        }

        private void OpenRouteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RouteList.SelectedItem != null;
        }

        private void OpenRoute(object sender, RoutedEventArgs e)
        {
            _selectedRouteId = ((RouteViewModel)RouteList.SelectedItem).Id;
            Close();
        }
    }
}
