using IoTEnergo.BL.ViewModels.Base;
using IoTEnergo.DAL.Services;
using IoTEnergo.DAL.Services.Abstract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTEnergo.BL.ViewModels.Map
{
    public class MapViewModel : ViewModelBase
    {
        private readonly IDeviceService _deviceService;

        private CancellationTokenSource cts = new CancellationTokenSource();

        public MapViewModel(IDeviceService deviceService)
        {
            _deviceService = deviceService;
            //_deviceService.DeviceRecievedMessage += RecievedMessage;
            MessageReceiver.RecievedMessage += RecievedMessage;
            //Task.Factory.StartNew(async () => await _deviceService.GetAll(cts));
        }

        private void Unsubscribe()
        {
            MessageReceiver.RecievedMessage -= RecievedMessage;
            //_deviceService.DeviceRecievedMessage -= RecievedMessage;
            //_deviceService.Unsubscribe();
        }

        private void RecievedMessage(string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                var devWSResponse = JsonConvert.DeserializeObject<DevicesResp>(message);
                if (devWSResponse != null && devWSResponse.cmd == "get_devices_resp")
                {
                    Debug.WriteLine(String.Format("get_devices_req status is {0}", devWSResponse.status));
                    if (devWSResponse.status == true)
                    {
                        Unsubscribe();
                        var objs = devWSResponse.devices_list;
                        //Device.BeginInvokeOnMainThread(() => Application.Current.MainPage = new AppShell());
                        //Unsubscribe();
                    }
                    //else
                        //Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.DisplayAlert("Authorization failed", "Incorrect Name or Password", "Cancel"));
                }
            }
        }
    }
}
