using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using DSIO.Modality.Api.Sdk.Types.V1;

namespace WpfSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();

            // Update title with version info
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            Title = $"{fileVersionInfo.ProductName} v{fileVersionInfo.ProductVersion}";
        }

        public MainViewModel ViewModel => DataContext as MainViewModel;

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            // Don't forget to delete your session before you leave!
            ViewModel.DeleteSession();
        }

        private void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            // Try to login with credentials
            ViewModel.Login();
        }

        private void BtnUpdateDevices_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdateDeviceList();
        }

        private void ComboDevices_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Change device used in session when selected device changes
            if (ComboDevices.SelectedItem is DeviceInfo device)
            {
                // If we have a session, switch the device to the selected one
                if (ViewModel.Session != null)
                {
                    ViewModel.ChangeDeviceForSession(device.DeviceId);
                }
            }
        }

        private void BtnCreateSession_OnClick(object sender, RoutedEventArgs e)
        {
            // delete previous session
            ViewModel.DeleteSession();

            // Create a new session
            ViewModel.CreateSession(ViewModel.SelectedDevice?.DeviceId);
        }

        private void BtnGetSessions_OnClick(object sender, RoutedEventArgs e)
        {
            // retrieve and show all session
            ViewModel.ShowActiveSessions();
        }

        private void BtnDeleteSession_OnClick(object sender, RoutedEventArgs e)
        {
            // delete session
            ViewModel.DeleteSession();
        }

        private void BtnUpdateAcquisitionInfo_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdateAcquisitionInfo();
        }
    }
}
