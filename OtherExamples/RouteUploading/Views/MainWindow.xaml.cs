using System;
using System.Windows;
using System.Windows.Navigation;
using UgCS.SDK.Examples.Common;
using UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.ViewModels;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _dataContext;
        private UcsFacade _ucs;


        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _ucs = await UcsFacade.connectToUcsAsync("localhost", 3334, "admin", "admin");
            }
            catch (Exception err)
            {
                MessageBox.Show(
                $"Unable to connect to UgCS server: '{err.Message}'.",
                "Connection error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
                Close();
                return;
            }


            _ucs.Disconnected += ucs_Disconnected;
            _dataContext = new MainWindowViewModel(_ucs);
            DataContext = _dataContext;
            MainFrame.Navigate(new Uri("Views/MainScreen.xaml", UriKind.Relative));
        }

        private void ucs_Disconnected(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Connection with UgCS server lost. The app will be closed.",
                "Connection error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            App.Current.Dispatcher.Invoke(() => Close());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_ucs != null)
                _ucs.Dispose();

            if (_dataContext != null)
                _dataContext.Dispose();
        }

        private void MainFrame_LoadCompleted(object sender, NavigationEventArgs e)
        {
            ((FrameworkElement)e.Content).DataContext = _dataContext;
        }
    }
}
