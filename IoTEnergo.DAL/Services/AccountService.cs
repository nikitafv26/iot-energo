using IoTEnergo.DAL.Models;
using IoTEnergo.DAL.Services.Abstract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static IoTEnergo.DAL.Services.RequestProvider;

namespace IoTEnergo.DAL.Services
{
    public class TokenAuthReq : WSRequest
    {
        public string token { get; set; }
    }

    public class AuthReq : WSRequest
    {
        public string login { get; set; }
        public string password { get; set; }
    }

    public class AccountService : IAccountService
    {
        private readonly IRequestProvider _requestProvider;

        public AccountService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
            //_requestProvider.RecievedMessage += RecievedMessage;
        }

        //public event RecievedMessageEventHandler AuthRecievedMessage;

        //private void RecievedMessage(string message)
        //{
        //    Debug.WriteLine(message);
        //    //AuthRecievedMessage(message);
        //}

        public async Task TokenOpen(string token, CancellationTokenSource cts)
        {
            var authTokenReq = new TokenAuthReq
            {
                cmd = "token_auth_req",
                token = token
            };

            var msg = JsonConvert.SerializeObject(authTokenReq);

            if (_requestProvider.State != System.Net.WebSockets.WebSocketState.Open)
                await _requestProvider.ConnectAsync(cts);

            await _requestProvider.SendAsync(msg, cts);
        }

        public async Task TokenClose(string token, CancellationTokenSource cts)
        {
            var authTokenReq = new TokenAuthReq
            {
                cmd = "close_auth_req",
                token = token
            };

            var msg = JsonConvert.SerializeObject(authTokenReq);

            if (_requestProvider.State != System.Net.WebSockets.WebSocketState.Open)
                await _requestProvider.ConnectAsync(cts);

            await _requestProvider.SendAsync(msg, cts);
        }

        public async Task Login(UserModel user, CancellationTokenSource cts)
        {
            var authReq = new AuthReq
            {
                cmd = "auth_req",
                login = user.Name,
                password = user.Password
            };

            var msg = JsonConvert.SerializeObject(authReq);

            if (_requestProvider.State != System.Net.WebSockets.WebSocketState.Open)
                await _requestProvider.ConnectAsync(cts);

            await _requestProvider.SendAsync(msg, cts);
        }

        public void Unsubscribe()
        {
            //_requestProvider.RecievedMessage -= RecievedMessage;
        }
    }
}
