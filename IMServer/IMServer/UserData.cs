using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMServer
{   
    // Can be saved into a file.
    [Serializable]
    class UserData
    {
        public string UserName;
        public string Password;

        // Saves user connectio status.
        [NonSerialized] public bool isLoggedIn;

        // Connection info
        [NonSerialized] public ClientHandler clientHandler;
        
        // User Record
        public UserData(string user, string pass, ClientHandler clientHandler)
        {
            this.UserName = user;
            this.Password = pass;
            this.isLoggedIn = true;
            this.clientHandler = clientHandler;
        }

        // User Record
        public UserData(string user, string pass)
        {
            this.UserName = user;
            this.Password = pass;
            this.isLoggedIn = false;
        }
    }    
}
