using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace IMServer
{   
    // This Class represent the instant messange TCP server. 
    class StartServer
    {
        // Saving user 
        string usersDataFile = Environment.CurrentDirectory + "\\users.dat";
        
        // IP of this computer. If you are running all clients at the same computer you can use 127.0.0.1 (localhost). 
        public IPAddress ipAdress = IPAddress.Parse("127.0.0.1");
        
        // Port number that server listen to. 
        public int portNum = 6000;
        
        // Tcp listner for clients connection.
        public TcpListener tcpServer;
        
        // Is server still running.
        public bool isRunning = true;
       
        // Registered Users Information
        public Dictionary<string, UserData> users = new Dictionary<string, UserData>();


        public StartServer()
        {
            Console.Title = "Instant Message TCP Server";
            Console.WriteLine("Initializing Server.....\n\n");

            // Loading old users from DB.
            LoadUsersFromFile();
            Console.WriteLine("[{0}]: Initializing server...", DateTime.Now);

            // Server linten to localhost in port 6000.
            tcpServer = new TcpListener(ipAdress, portNum);

            // Start the server
            tcpServer.Start();
            Console.WriteLine("[{0}]: Server is running", DateTime.Now);
            
            // Listen to all incoming connection while server is running.
            while (isRunning)
            {
                TcpClient tcpClient = this.tcpServer.AcceptTcpClient();  // Accept incoming connection.
                ClientHandler client = new ClientHandler(this, tcpClient);     // Handle in another thread.
            }

        }

       ///  Save The users into the file
        public void SaveUsersToFile()  
        {
            try
            {
                Console.WriteLine("[{0}]: Saving users to DB", DateTime.Now);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                
                // Open new file.
                FileStream file = new FileStream(usersDataFile, FileMode.Create, FileAccess.Write);
                
                // Saving users into file in derialize mode.
                binaryFormatter.Serialize(file, users.Values.ToArray());  
                file.Close();
                Console.WriteLine("[{0}]: Users saved to DB!", DateTime.Now);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // Load Users from DB file.
        public void LoadUsersFromFile()  
        {
            try
            {
                Console.WriteLine("[{0}]: Loading users from DB", DateTime.Now);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream file = new FileStream(usersDataFile, FileMode.Open, FileAccess.Read);

                // Loadin users from DB.
                UserData[] usersData = (UserData[])binaryFormatter.Deserialize(file);      
                file.Close();
                users = usersData.ToDictionary((user) => user.UserName, (user) => user); 
                Console.WriteLine("[{0}]: Users loaded from DB ({1})", DateTime.Now, users.Count);
            }
            catch 
            {
            }
        }
    }
}
