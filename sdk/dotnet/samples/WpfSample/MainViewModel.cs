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
        private ISubscription _acquisitionStatusSubscription;

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

        #region Images

        private AcquisitionSession _session;
        public AcquisitionSession Session
        {
            get => _session;
            set
            {
                if (value != _session)
                {
                    _session = value;
                    OnPropertyChanged();
                }
            }
        }

        private AcquisitionInfo _acquisitionInfo;
        public AcquisitionInfo AcquisitionInfo
        {
            get => _acquisitionInfo;
            set
            {
                if (value != _acquisitionInfo)
                {
                    _acquisitionInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        private AcquisitionStatus _acquisitionStatus;
        public AcquisitionStatus AcquisitionStatus
        {
            get => _acquisitionStatus;
            set
            {
                if (value != _acquisitionStatus)
                {
                    _acquisitionStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ImageInfo> _images;

        public ObservableCollection<ImageInfo> Images
        {
            get => _images;
            set
            {
                if (value != _images)
                {
                    _images = value;
                    OnPropertyChanged();
                }
            }
        }

        private ImageInfo _selectedImage;

        public ImageInfo SelectedImage
        {
            get => _selectedImage;
            set
            {
                if (value != _selectedImage)
                {
                    _selectedImage = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task<AcquisitionSession> CreateSession(string deviceId)
        {
            var sessionInfo = new AcquisitionSessionInfo
            {
                ClientName = "WpfSample",
                DeviceId = deviceId
            };

            Session = await _serviceProxy.CreateAcquisitionSession(sessionInfo);
            if (Session != null)
            {
                // subscribe to Acquisition status
                _acquisitionStatusSubscription = await _serviceProxy.SubscribeToAcquisitionStatus(Session.SessionId,
                    ProcessAcquisitionStatus);
                _acquisitionStatusSubscription?.Start();

                // Initialize current status
                AcquisitionStatus = await _serviceProxy.GetAcquisitionStatus(Session.SessionId);
                ProcessAcquisitionStatus(AcquisitionStatus);

            }
            return Session;
        }

        public async Task<AcquisitionSession> ChangeDeviceForSession(string deviceId)
        {
            if (Session != null)
            {
                var sessionInfo = new AcquisitionSessionInfo
                {
                    ClientName = Session.ClientName,
                    DeviceId = deviceId
                };

                Session = await _serviceProxy.UpdateAcquisitionSession(Session.SessionId, sessionInfo);
            }

            return Session;
        }

        public async Task<bool> DeleteSession()
        {
            if (Session != null)
            {
                // Unsubscribe status
                _acquisitionStatusSubscription?.Stop();
                _acquisitionStatusSubscription = null;

                var result = await _serviceProxy.DeleteAcquisitionSession(Session.SessionId);
                Session = null;
                return result;
            }

            return true;
        }

        public async Task<AcquisitionInfo> UpdateAcquisitionInfo()
        {
            if (Session != null)
            {
                AcquisitionInfo = await _serviceProxy.UpdateAcquisitionInfo(Session.SessionId, AcquisitionInfo);
            }

            return AcquisitionInfo;
        }

        public async Task<ObservableCollection<ImageInfo>> UpdateImages()
        {
            if (Session != null)
            {
                var images = await _serviceProxy.GetAllImages(Session.SessionId);
                Images = new ObservableCollection<ImageInfo>(images);
            }

            return Images;
        }

        #endregion

        private void ProcessAcquisitionStatus(AcquisitionStatus status)
        {
            AcquisitionStatus = status;
            if (status.State == AcquisitionStatus.AcquisitionState.NoAcquisitionInfo)
            {
                // Create a new instance of AcquisitionInfo so we can fill in properties
                AcquisitionInfo = new AcquisitionInfo();
            }
            else if (status.State == AcquisitionStatus.AcquisitionState.NewImage ||
                     status.TotalImages != Images?.Count)
            {
                // New images arrived, update Images list
                UpdateImages();
            }
        }
    }
}
