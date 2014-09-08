using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMClient
{   

    // Handles client error events.
    public class ClientErrorEvents : EventArgs
    {
        ClientErrorsEnum errorTypes;

        public ClientErrorEvents(ClientErrorsEnum error)
        {
            this.errorTypes = error;
        }

        public ClientErrorsEnum Error
        {
            get { return errorTypes; }
        }

    }
    public delegate void ClientErrorEventHandler(object sender, ClientErrorEvents e);
}