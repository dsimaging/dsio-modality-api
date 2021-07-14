using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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

        public void Login()
        {
            // Update status
            LoginStatus = "Logging in...";
            IsAuthenticated = false;

            // configure credentials
            _serviceProxy.SetBasicAuthenticationHeader(ApiUserName, ApiKey);

            // Check availability to confirm credentials
            _serviceProxy.IsServiceAvailable().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // Show the error message
                    LoginStatus = "Error";
                    MessageBox.Show(task.Exception?.Message);
                }
                else if (task.IsCompleted)
                {
                    IsAuthenticated = task.Result;
                    LoginStatus = IsAuthenticated ? "Login Successful" : "Service is unavailable";
                }
            });
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

        // Must be called from UI thread to properly update Devices list and Selected Device
        public void UpdateDeviceList()
        {
            // Disable current selection
            SelectedDevice = null;
            Mouse.OverrideCursor = Cursors.Wait;

            // Get all devices
            _serviceProxy.GetAllDevices().ContinueWith(task =>
            {
                // Must be on UI thread to change Mouse
                Mouse.OverrideCursor = null;
                if (task.IsFaulted)
                {
                    MessageBox.Show(task.Exception?.Message);
                }
                else if (task.IsCompletedSuccessfully)
                {
                    var devices = task.Result;
                    Devices = new ObservableCollection<DeviceInfo>(devices);

                    // Select first device by default
                    SelectedDevice = Devices.FirstOrDefault();
                }
                // We synchronize the Continuation task so we can make UI changes
            }, TaskScheduler.FromCurrentSynchronizationContext());
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

                    // Update the image bitmap when the selected image changes
                    SelectedImageBitmap = null;
                    if (_selectedImage != null)
                    {
                        UpdateSelectedImageBitmap(_selectedImage.Id);
                    }

                    OnPropertyChanged();
                }
            }
        }

        private BitmapSource _selectedImageBitmap;

        public BitmapSource SelectedImageBitmap
        {
            get => _selectedImageBitmap;
            set
            {
                if (value != _selectedImageBitmap)
                {
                    _selectedImageBitmap = value;
                    OnPropertyChanged();
                }
            }
        }

        public void CreateSession(string deviceId)
        {
            DeleteSession();

            var sessionInfo = new AcquisitionSessionInfo
            {
                ClientName = "WpfSample",
                DeviceId = deviceId
            };

            // Create the session
            _serviceProxy.CreateAcquisitionSession(sessionInfo).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    MessageBox.Show(task.Exception?.Message);
                }
                else if (task.IsCompleted)
                {
                    Session = task.Result;
                    if (Session != null)
                    {
                        // subscribe to Acquisition status
                        _serviceProxy.SubscribeToAcquisitionStatus(Session.SessionId, ProcessAcquisitionStatus)
                            .ContinueWith(subTask =>
                            {
                                _acquisitionStatusSubscription = subTask.Result;
                                _acquisitionStatusSubscription?.Start();
                            });

                        // Initialize current status
                        _serviceProxy.GetAcquisitionStatus(Session.SessionId)
                            .ContinueWith(subTask => ProcessAcquisitionStatus(subTask.Result));
                    }
                }
            });
        }

        public void ChangeDeviceForSession(string deviceId)
        {
            // Switch the device used for this session
            if (Session != null)
            {
                var sessionInfo = new AcquisitionSessionInfo
                {
                    ClientName = Session.ClientName,
                    DeviceId = deviceId
                };

                _serviceProxy.UpdateAcquisitionSession(Session.SessionId, sessionInfo)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            MessageBox.Show(task.Exception?.Message);
                        }
                        else if (task.IsCompleted)
                        {
                            Session = task.Result;
                        }
                    });
            }
        }

        public void DeleteSession()
        {
            if (Session != null)
            {
                // Unsubscribe status
                _acquisitionStatusSubscription?.Stop();
                _acquisitionStatusSubscription = null;

                _serviceProxy.DeleteAcquisitionSession(Session.SessionId)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            MessageBox.Show(task.Exception?.Message);
                        }
                    });

                // Cleanup session data
                Session = null;
                AcquisitionInfo = null;
                AcquisitionStatus = null;
                SelectedImage = null;
                Images = null;
            }
        }

        public void UpdateAcquisitionInfo()
        {
            // Set AcquisitionInfo for the next exposure using the
            // AcquisitionInfo property of our ViewModel
            if (Session != null && AcquisitionInfo != null)
            {
                _serviceProxy.UpdateAcquisitionInfo(Session.SessionId, AcquisitionInfo)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            MessageBox.Show(task.Exception?.Message);
                        }
                        else if (task.IsCompleted)
                        {
                            AcquisitionInfo = task.Result;
                        }
                    });
            }
        }

        // Must be called from UI thread to properly update Images and SelectedImage
        public void UpdateImageList(string selectImageId = null)
        {
            // Update our collection of images
            if (Session != null)
            {
                _serviceProxy.GetAllImages(Session.SessionId)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            MessageBox.Show(task.Exception?.Message);
                        }
                        else if (task.IsCompleted)
                        {
                            Images = new ObservableCollection<ImageInfo>(task.Result);
                            
                            // try to reselect the SelectedImage
                            if (!string.IsNullOrEmpty(selectImageId))
                                SelectedImage = Images.FirstOrDefault(info => info.Id == selectImageId);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        #endregion

        /// <summary>
        /// This callback is used to process current AcquisitionStatus from our subscription
        /// </summary>
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
                // New image arrived, update Images list (must be called from UI thread)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Update the list and show latest image
                    UpdateImageList(status.LastImageId);
                });
            }
        }

        private void UpdateSelectedImageBitmap(string imageId)
        {
            SelectedImageBitmap = null;
            if (Session != null)
            {
                _serviceProxy.GetImageMedia(Session.SessionId, imageId)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            MessageBox.Show(task.Exception?.Message);
                        }
                        else if (task.IsCompleted)
                        {
                            // Create a Bitmap image from the stream
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.StreamSource = task.Result;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            bitmap.Freeze();

                            SelectedImageBitmap = bitmap;
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
    }
}
