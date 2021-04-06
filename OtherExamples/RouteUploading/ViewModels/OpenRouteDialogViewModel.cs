using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.ViewModels
{
    internal sealed class OpenRouteDialogViewModel
    {
        public class RouteViewModel
        {
            public int Id { get; private set; }

            public string FullName { get; private set; }


            public RouteViewModel(int id, string fullName)
            {
                Id = id;
                FullName = fullName;
            }

        }

        public RouteViewModel SelectedRoute { get; set; }


        public ObservableCollection<RouteViewModel> Routes { get; private set; }

        public OpenRouteDialogViewModel()
        {
            Routes = new ObservableCollection<RouteViewModel>();
        }
    }
}
