using IoTEnergo.BL.ViewModels.Base;
using IoTEnergo.DAL.Models;
using IoTEnergo.DAL.Services;
using IoTEnergo.DAL.Services.Abstract;
using IoTEnergo.UI.Pages.Account;
using IoTEnergo.UI.Pages.Chart;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IoTEnergo.BL.ViewModels.Device
{
    public class DeviceViewModel : ViewModelBase
    {
        private bool _isRefreshing;
        private readonly IDeviceService _deviceService;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private IAccountService _accountService;

        public DeviceViewModel(IDeviceService deviceService, IAccountService accountService)
        {
            _accountService = accountService;
            _deviceService = deviceService;
            //_deviceService.DeviceRecievedMessage += RecievedMessage;
            Subscribe();
            Task.Factory.StartNew(async () =>
            {
                await AutoLogin();
                await GetDevices();
            });
        }

        private async Task AutoLogin()
        {
            string name = Preferences.Get("Login", String.Empty);
            string password = Preferences.Get("Password", String.Empty);

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(password))
            {
                var user = new UserModel { Name = name, Password = password };
                try
                {
                    await _accountService.Login(user, cts);
                }
                catch (SocketException ex)
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.DisplayAlert("Authorization failed", ex.Message, "Cancel"));
                }
            }
        }

        private async Task GetDevices()
        {
            try
            {
                await _deviceService.GetAll(cts);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);
            }
        }

        private void Unsubscribe()
        {
            MessageReceiver.RecievedMessage -= RecievedMessage;
        }

        private void Subscribe()
        {
            MessageReceiver.RecievedMessage += RecievedMessage;
        }

        private void RecievedMessage(string message)
        {
            try
            {
                if (!String.IsNullOrEmpty(message))
                {
                    var devWSResponse = JsonConvert.DeserializeObject<DevicesResp>(message);

                    if (devWSResponse.err_string == "unknown_auth" || devWSResponse.err_string == "invalid_token")
                    {
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            Shell.Current.Navigation.PushModalAsync(new LoginPage());
                        });
                        Unsubscribe();
                    }

                    if (devWSResponse != null && devWSResponse.cmd == "get_devices_resp")
                    {
                        Debug.WriteLine(String.Format("get_devices_req status is {0}", devWSResponse.status));
                        if (devWSResponse.status == true)
                        {
                            Unsubscribe();

                            var devices = devWSResponse.devices_list.Select(resp => new DeviceModel
                            {
                                Id = resp.devEui,
                                Name = resp.devName,
                                Location = new LocationModel
                                {
                                    Latitude = resp.position.latitude,
                                    Longitude = resp.position.longitude,
                                    Altitude = resp.position.altitude
                                }
                            });

                            if (Devices.Count > 0)
                                Devices.Clear();

                            foreach (var dev in devices)
                                Devices.Add(dev);
                        }
                        //else
                        //Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.DisplayAlert("Authorization failed", "Incorrect Name or Password", "Cancel"));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public ObservableCollection<DeviceModel> Devices { get; set; } = new ObservableCollection<DeviceModel>();

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    RaisePropertyChanged(() => IsRefreshing);
                }
            }
        }

        public ICommand DeviceTappedCommand => new Command(async (o) =>
        {
            if (o is DeviceModel selectedDevice)
            {
                string id = ((DeviceModel)o).Id;
                SettingsViewModel.Instance.Id = id;
                await Shell.Current.GoToAsync($"//device/chart?id={id}", true);
            }
        });

        public ICommand RefreshCommand => new Command(async () =>
        {
            IsRefreshing = true;
            Subscribe();
            await GetDevices();
            IsRefreshing = false;
        });
    }
}
