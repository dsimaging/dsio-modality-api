using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DSIO.Modality.Api.Sdk.Client.V1;

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
            _serviceProxy.SetBasicAuthenticationHeader(ApiUserName, ApiKey);

            // Check availability to confirm credentials
            try
            {
                var isGood = await _serviceProxy.IsServiceAvailable();
                LoginStatus = isGood ? "Login Successful" : "Service is unavailable";
                return isGood;
            }
            catch (Exception)
            {
                LoginStatus = "Error";
                throw;
            }
        }
    }
}
