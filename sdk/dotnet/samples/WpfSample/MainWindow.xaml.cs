using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        }

        public MainViewModel ViewModel => DataContext as MainViewModel;

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            // Don't forget to delete your session before you leave!
            if (ViewModel.Session != null)
            {
                ViewModel.DeleteSession()
                    .ContinueWith(task =>
                    {
                        // Display error message if exception occurred
                        if (task.IsFaulted)
                        {
                            MessageBox.Show(task.Exception?.Message, "Delete Session Error");
                        }
                    });
            }
        }

        private void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            // Try to login with credentials
            ViewModel.Login(ViewModel.ApiUserName, ViewModel.ApiKey)
                .ContinueWith(task =>
                {
                    // Display error message if exception occurred
                    if (task.IsFaulted)
                    {
                        MessageBox.Show(task.Exception?.Message,"Login Error");
                    }
                });
        }

        private void BtnUpdateDevices_OnClick(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            ViewModel.UpdateDeviceList()
                .ContinueWith(task =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Mouse.OverrideCursor = null;
                    });
                    // Display error message if exception occurred
                    if (task.IsFaulted)
                    {
                        MessageBox.Show(task.Exception?.Message, "Update Devices Error");
                    }
                });
        }

        private void ComboDevices_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update sensor info when selected device changes
            if (ComboDevices.SelectedItem is DeviceInfo device)
            {
                ViewModel.SelectedSensor = null;
                ViewModel.UpdateSensorInfo(device.DeviceId)
                    .ContinueWith(task =>
                    {
                        // Display error message if exception occurred
                        if (task.IsFaulted)
                        {
                            MessageBox.Show(task.Exception?.Message, "Update Sensor Error");
                        }
                    });

                // If we have a session, switch the device to the selected one
                if (ViewModel.Session != null)
                {
                    ViewModel.ChangeDeviceForSession(device.DeviceId)
                        .ContinueWith(task =>
                        {
                            // Display error message if exception occurred
                            if (task.IsFaulted)
                            {
                                MessageBox.Show(task.Exception?.Message, "Update Session Error");
                            }
                        });
                }
            }
        }

        private void BtnCreateSession_OnClick(object sender, RoutedEventArgs e)
        {
            // delete previous session
            ViewModel.DeleteSession()
                .ContinueWith(task =>
                {
                    // Display error message if exception occurred
                    if (task.IsFaulted)
                    {
                        MessageBox.Show(task.Exception?.Message, "Delete Session Error");
                    }
                });

            if (ViewModel.SelectedDevice != null)
            {
                ViewModel.CreateSession(ViewModel.SelectedDevice.DeviceId)
                    .ContinueWith(task =>
                    {
                        // Display error message if exception occurred
                        if (task.IsFaulted)
                        {
                            MessageBox.Show(task.Exception?.Message, "Create Session Error");
                        }
                    });

            }
        }

        private void BtnDeleteSession_OnClick(object sender, RoutedEventArgs e)
        {
            // delete session
            ViewModel.DeleteSession()
                .ContinueWith(task =>
                {
                    // Display error message if exception occurred
                    if (task.IsFaulted)
                    {
                        MessageBox.Show(task.Exception?.Message, "Delete Session Error");
                    }
                });
        }

        private void BtnUpdateAcquisitionInfo_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Session != null)
            {
                ViewModel.UpdateAcquisitionInfo()
                    .ContinueWith(task =>
                    {
                        // Display error message if exception occurred
                        if (task.IsFaulted)
                        {
                            MessageBox.Show(task.Exception?.Message, "Update Acquisition Info Error");
                        }
                    });

            }
        }
    }
}
