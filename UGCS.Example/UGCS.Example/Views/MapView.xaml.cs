using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using MapControl;
using Ninject;
using UGCS.Example.ViewModels;

namespace UGCS.Example.Views
{
    /// <summary>
    /// Interaction logic for MapLayer.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        private MapViewModel mvm = App.Kernel.Get<MapViewModel>();
        public MapView()
        {
            InitializeComponent();
            map.ZoomLevel = 18;
            map.ManipulationMode = ManipulationModes.All;
            mvm.WPFMap = map;
        }

        private void MapManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 0.001;
        }

        private void MapItemTouchDown(object sender, TouchEventArgs e)
        {
            var mapItem = (MapItem)sender;
            mapItem.IsSelected = !mapItem.IsSelected;
            e.Handled = true;
        }
    }
}
