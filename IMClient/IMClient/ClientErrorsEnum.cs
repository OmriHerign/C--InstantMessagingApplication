using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMClient
{   
    // Enum error events. 
    public enum ClientErrorsEnum
    {
        TooUserName = 3,
        TooPassword = 4,
        Exists = 5,
        NoExists = 6,
        WrongPassword = 7
    }
}
