using IoTEnergo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static IoTEnergo.DAL.Services.RequestProvider;

namespace IoTEnergo.DAL.Services.Abstract
{
    public interface IDeviceService
    {
        Task GetAll(CancellationTokenSource cts);
        Task Get(string startDate, string endDate, string id, CancellationTokenSource cts);

        //event RecievedMessageEventHandler DeviceRecievedMessage;
        void Unsubscribe();
    }
}
