using System;
using System.Collections.Generic;
using System.Text;

namespace IoTEnergo.DAL.Services
{
    public class WSResponse
    {
        public string cmd { get; set; }
        public bool status { get; set; }
        public string token { get; set; }
        public string err_string { get; set; }
    }
}
