using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IMClient
{
    static class ClientRunner
    {
        // Run the client application.
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Run the login form.
            LoginForm newClient = new LoginForm();
            Application.Run(newClient);
        }
    }
}
