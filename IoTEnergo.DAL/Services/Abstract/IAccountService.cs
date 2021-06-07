using IoTEnergo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static IoTEnergo.DAL.Services.RequestProvider;

namespace IoTEnergo.DAL.Services.Abstract
{
    public interface IAccountService
    {
        Task Login(UserModel user, CancellationTokenSource cts);
        Task TokenOpen(string token, CancellationTokenSource cts);
        Task TokenClose(string token, CancellationTokenSource cts);
        //event RecievedMessageEventHandler AuthRecievedMessage;
        void Unsubscribe();
    }
}
