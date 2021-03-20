using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DSIO.Modality.Api.Sdk.Client.V1;
using DSIO.Modality.Api.Sdk.Types.V1;

namespace WpfSample
{
    /// <summary>
    /// ViewModel class for MainWindow
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ServiceProxy _serviceProxy;

        public MainViewModel()
        {
            _serviceProxy = new ServiceProxy();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Login

        private string _apiUserName;
        public string ApiUserName
        {
            get => _apiUserName;
            set
            {
                if (value != _apiUserName)
                {
                    _apiUserName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _apiKey;
        public string ApiKey
        {
            get => _apiKey;
            set
            {
                if (value != _apiKey)
                {
                    _apiKey = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isAuthenticated;
        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            private set
            {
                if (value != _isAuthenticated)
                {
                    _isAuthenticated = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _loginStatus;
        public string LoginStatus
        {
            get => _loginStatus;
            set
            {
                if (value != _loginStatus)
                {
                    _loginStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task<bool> Login(string username, string apikey)
        {
            // set credentials
            ApiUserName = username;
            ApiKey = apikey;
            LoginStatus = "Logging in...";
            IsAuthenticated = false;
            _serviceProxy.SetBasicAuthenticationHeader(ApiUserName, ApiKey);

            // Check availability to confirm credentials
            try
            {
                IsAuthenticated = await _serviceProxy.IsServiceAvailable();
                LoginStatus = IsAuthenticated ? "Login Successful" : "Service is unavailable";
                return IsAuthenticated;
            }
            catch (Exception)
            {
                IsAuthenticated = false;
                LoginStatus = "Error";
                throw;
            }
        }

        #endregion

        #region Device

        private ObservableCollection<DeviceInfo> _devices;

        public ObservableCollection<DeviceInfo> Devices
        {
            get => _devices;
            private set
            {
                _devices = value;
                OnPropertyChanged();
            }
        }

        private DeviceInfo _selectedDevice;
        public DeviceInfo SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (value != _selectedDevice)
                {
                    _selectedDevice = value;
                    OnPropertyChanged();
                }
            }
        }

        private SensorInfo _selectedSensor;
        public SensorInfo SelectedSensor
        {
            get => _selectedSensor;
            set
            {
                if (value != _selectedSensor)
                {
                    _selectedSensor = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task<ObservableCollection<DeviceInfo>> UpdateDeviceList()
        {
            SelectedDevice = null;
            SelectedSensor = null;

            // Get all devices
            var devices = await _serviceProxy.GetAllDevices();
            Devices = new ObservableCollection<DeviceInfo>(devices);
            return Devices;
        }

        public async Task<DeviceInfo> UpdateDeviceInfo(string deviceId)
        {
            // Get detailed info for device
            SelectedDevice = await _serviceProxy.GetDevice(deviceId);
            return SelectedDevice;
        }

        public async Task<SensorInfo> UpdateSensorInfo(string deviceId)
        {
            // Get detailed info for sensor
            SelectedSensor = await _serviceProxy.GetSensor(deviceId);
            return SelectedSensor;
        }

        #endregion
    }
}
