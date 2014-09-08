using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMClient
{   
    // Handles client recived information events.
    public class ClientReceivedMessagesEvents : EventArgs
    {
        string userName;
        string message;

        public ClientReceivedMessagesEvents(string i_userName, string i_mssage)
        {
            this.userName = i_userName;
            this.message = i_mssage;
        }

        public string FromUserName
        {
            get { return userName; }
        }
        public string MessageRecived
        {
            get { return message; }
        }
    }
    public delegate void ClientReceivedMessageEventHandler(object sender, ClientReceivedMessagesEvents e);
}
