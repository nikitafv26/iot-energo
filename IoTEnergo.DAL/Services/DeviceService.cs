using IoTEnergo.DAL.Models;
using IoTEnergo.DAL.Services.Abstract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static IoTEnergo.DAL.Services.RequestProvider;

namespace IoTEnergo.DAL.Services
{
    public class DataReq : WSRequest
    {
        public class Select
        {
            public long date_from { get; set; }
            public long date_to { get; set; }
            public int limit { get; set; }
        }

        public string devEui { get; set; }
        public Select select { get; set; }
    }

    public class DataResp : WSResponse
    {
        public class Data
        {
            public string data { get; set; }
        }

        public List<Data> data_list { get; set; }
    }

    public class ExportData
    {
        public string DeviceId { get; set; }
        public string Date { get; set; }
        public string Value { get; set; }
    }

    public class DevicesResp : WSResponse
    {
        public class Position
        {
            public double longitude { get; set; }
            public double latitude { get; set; }
            public double altitude { get; set; }
        }

        public class Device
        {
            public string devEui { get; set; }
            public string devName { get; set; }
            public Position position { get; set; }
        }

        public List<Device> devices_list { get; set; }
    }

    public class DeviceService : IDeviceService
    {
        private readonly IRequestProvider _requestProvider;

        //public event RecievedMessageEventHandler DeviceRecievedMessage;

        public DeviceService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
            //_requestProvider.RecievedMessage += RecievedMessage;
        }

        private void RecievedMessage(string message)
        {
            //DeviceRecievedMessage(message);
        }

        public async Task Get(string startDate, string endDate, string id, CancellationTokenSource cts)
        {
            var dateFrom = DateTimeOffset.ParseExact(startDate, "yyyyMMdd", null).ToUnixTimeMilliseconds();
            var dateTo = DateTimeOffset.ParseExact(endDate, "yyyyMMdd", null).AddTicks(-1).AddDays(1).ToUnixTimeMilliseconds();

            //var dateFrom = DateTimeOffset.ParseExact(startDate + " 000000", "yyyyMMdd HHmmss", null).ToUnixTimeMilliseconds();
            //var dateTo = DateTimeOffset.ParseExact(endDate + " 235959", "yyyyMMdd HHmmss", null).AddDays(1).ToUnixTimeMilliseconds();
            var request = new DataReq //get_data_req
            {
                cmd = "get_data_req",
                devEui = id,
                select = new DataReq.Select
                {
                    date_from = dateFrom,
                    date_to = dateTo,
                    limit = 10000
                }
            };

            var msg = JsonConvert.SerializeObject(request);

            if (_requestProvider.State != System.Net.WebSockets.WebSocketState.Open)
                await _requestProvider.ConnectAsync(cts);

            await _requestProvider.SendAsync(msg, cts);
        }

        public async Task GetAll(CancellationTokenSource cts)
        {
            var request = new WSRequest { cmd = "get_devices_req" }; //get_devices_req

            var msg = JsonConvert.SerializeObject(request);

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
