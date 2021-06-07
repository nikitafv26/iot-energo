using IoTEnergo.DAL.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTEnergo.DAL.Services
{
    public static class MessageReceiver
    {
        public delegate void RecievedMessageEventHandler(string message);
        public static event RecievedMessageEventHandler RecievedMessage;

        public static RecievedMessageEventHandler RecievedMessageProp
        {
            get { return RecievedMessage; }
            set
            {
                if (RecievedMessage == null)
                    RecievedMessage = value;
            }
        }
    }
}
