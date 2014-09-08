using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMClient
{   

    // Availabilty events handler 
    public class ClientAvailableEvent : EventArgs
    {
        string userName;
        bool isAvailable;

        public ClientAvailableEvent(string i_userName, bool i_isAvailable)
        {
            this.userName = i_userName;
            this.isAvailable = i_isAvailable;
        }

        public string UserName
        {
            get 
            { 
                return this.userName;
            }
        }

        public bool IsAvailable
        {
            get 
            { 
                return this.isAvailable;
            }
        }
    }
    public delegate void ClientAvailableEventsHandler(object sender, ClientAvailableEvent e);
}
