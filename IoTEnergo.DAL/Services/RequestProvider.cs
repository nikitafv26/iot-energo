using IoTEnergo.DAL.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTEnergo.DAL.Services
{
    public class RequestProvider : IRequestProvider
    {
        private static ClientWebSocket client;

        //public delegate void RecievedMessageEventHandler(string message);
        //public event RecievedMessageEventHandler RecievedMessage;

        public RequestProvider()
        {
            if (client == null)
                client = GetClient();
        }

        public WebSocketState State => client != null ? client.State : WebSocketState.None;

        public async Task ConnectAsync(CancellationTokenSource cts)
        {
            try
            {
                await client.ConnectAsync(new Uri("ws://10.2.0.186:8555"), cts.Token); //local 10.2.0.217:8002 //10.2.6.119:8555

                await Task.Factory.StartNew(async () =>
                {
                    while (client.State == WebSocketState.Open)
                    {
                        await ReadMessage(cts);
                    }
                }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SocketException)
                {
                    var se = (SocketException)ex.InnerException;
                    if (se.ErrorCode == 10051)
                    {
                        throw new SocketException(10051);
                    }
                }
            }
        }

        private async Task ReadMessage(CancellationTokenSource cts)
        {
            try
            {
                WebSocketReceiveResult result;
                var message = new ArraySegment<byte>(new byte[524288]);
                do
                {
                    result = await client.ReceiveAsync(message, cts.Token);
                    if (result.MessageType != WebSocketMessageType.Text)
                        break;
                    var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                    string receivedMessage = Encoding.UTF8.GetString(messageBytes);
                    MessageReceiver.RecievedMessageProp(receivedMessage);
                    //Console.WriteLine("Received: {0}", receivedMessage);
                }
                while (!result.EndOfMessage || !cts.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task SendAsync(string message, CancellationTokenSource cts)
        {
            try
            {
                var byteMessage = Encoding.UTF8.GetBytes(message);
                var segmnet = new ArraySegment<byte>(byteMessage);

                if (client.State == WebSocketState.Open)
                    await client.SendAsync(segmnet, WebSocketMessageType.Text, true, cts.Token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task CloseAsync()
        {
            try
            {
                
                if (client.State == WebSocketState.Open)
                {
                    await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                if (client.State == WebSocketState.CloseReceived)
                {
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    if (client != null)
                    {
                        client.Dispose();
                        client = null;
                    }
                }
                if (client.State == WebSocketState.Closed || client.State == WebSocketState.Aborted)
                {
                    if (client != null)
                    {
                        client.Dispose();
                        client = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private ClientWebSocket GetClient()
        {
            return new ClientWebSocket();
        }
    }
}
