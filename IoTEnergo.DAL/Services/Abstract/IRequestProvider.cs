using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static IoTEnergo.DAL.Services.RequestProvider;

namespace IoTEnergo.DAL.Services.Abstract
{
    public interface IRequestProvider
    {
        WebSocketState State { get; }
        //event RecievedMessageEventHandler RecievedMessage;
        Task ConnectAsync(CancellationTokenSource cts);
        Task SendAsync(string message, CancellationTokenSource cts);
        Task CloseAsync();
    }
}
