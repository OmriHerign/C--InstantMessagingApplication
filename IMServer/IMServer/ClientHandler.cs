using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.Threading;

namespace IMServer
{   
    // This class handles the connection of several users.
    class ClientHandler
    {
        // Connections Mode.
        public const int ESTABLISH_CONNECTION = 6600;      
        public const byte CONNECTED = 0;          
        public const byte LOGIN_MODE = 1;        
        public const byte REGISTER_MODE = 2;     
        public const byte USER_EXISTS = 3;      
        public const byte USER_DOES_NOT_EXISTS = 4;    
        public const byte WRONG_PASSWORD = 5;   
        public const byte USER_AVAILABILITY_CHECK = 6; 
        public const byte SEND_MESSAGE = 7;         
        public const byte MESSAGE_RECIVED = 8;
        
        // Save the server details.
        StartServer myServer;

        // Tcp client for connection handeling.
        public TcpClient tcpClient;

        // Network stream over connection.
        public NetworkStream networkStream;  // Raw-data stream of connection.
        
        // Binary format for reading and writing.
        public BinaryReader binaryReader;
        public BinaryWriter binaryWriter;
        
        // Current User info.
        UserData userDeatails; 
        
        // Handle connection from client.
        public ClientHandler(StartServer i_server, TcpClient i_tcpClient)
        {
            this.myServer = i_server;
            this.tcpClient = i_tcpClient;

            // Handle client in another thread.
            (new Thread(new ThreadStart(StartConnection))).Start();
        }

        // Setup connection and login or register.
        void StartConnection()  
        {
            try
            {
                Console.WriteLine("[{0}] New connection had been established", DateTime.Now);
                
                // Recive network stream.
                networkStream = tcpClient.GetStream();

                // Encoding steam into UTF8.
                binaryReader = new BinaryReader(networkStream, Encoding.UTF8);
                binaryWriter = new BinaryWriter(networkStream, Encoding.UTF8);

                // Connection signal over the stream.
                binaryWriter.Write(ESTABLISH_CONNECTION);
                binaryWriter.Flush();
                
                // Answer from client.
                int connectionSignal = binaryReader.ReadInt32();
                
                // If connection succeded. 
                if (connectionSignal == ESTABLISH_CONNECTION)
                {
                    byte connectionMode = binaryReader.ReadByte();
                    string userName = binaryReader.ReadString();
                    string password = binaryReader.ReadString();

                    // If we are dealing with register mode.
                    if (connectionMode == REGISTER_MODE)  
                    {   
                        // If user does not already exists.
                        if (!myServer.users.ContainsKey(userName))
                        {
                            
                            // Update users DB.
                            userDeatails = new UserData(userName, password, this);
                            myServer.users.Add(userName, userDeatails);  // Add new user
                            
                            // Confirm connection to client.
                            binaryWriter.Write(CONNECTED);
                            binaryWriter.Flush();
                            Console.WriteLine("[{0}] {1} Just registered as new user to chat", DateTime.Now, userName);
                            
                            // Saves users DB
                            myServer.SaveUsersToFile();

                            // Recive info from client.
                            InformationHandler();  
                        }
                        else
                            binaryWriter.Write(USER_EXISTS);
                    }

                    // If in login mode.
                    else if (connectionMode == LOGIN_MODE)
                    {
                        // Checks if user already exists.
                        if (myServer.users.TryGetValue(userName, out userDeatails))
                        {
                            // If password does not matched user name.
                            if (password == userDeatails.Password) 
                            {
                                // If user already connected disconnect him.
                                if (userDeatails.isLoggedIn)
                                {
                                    userDeatails.clientHandler.CloseConnection();
                                }

                                // Start connection.
                                userDeatails.clientHandler = this;
                                binaryWriter.Write(CONNECTED);
                                binaryWriter.Flush();
                                
                                // Listen to connection from users.
                                InformationHandler();  
                            }

                            // Wrong user password    
                            else
                            {
                                binaryWriter.Write(WRONG_PASSWORD);
                            }
                        }

                        // User does not exists.
                        else
                        {
                            binaryWriter.Write(USER_DOES_NOT_EXISTS);
                        }
                    }
                }

                // Close connection if not in login or register mode.
                CloseConnection();
            }
            catch {
 
                // Close connection if somethig goes wrong.
                CloseConnection(); 
            }
        }

        // Close connection.
        void CloseConnection() 
        {
            try
            {
                // Close all resourses and and change user mode.
                userDeatails.isLoggedIn = false;
                binaryReader.Close();
                binaryWriter.Close();
                networkStream.Close();
                tcpClient.Close();
                Console.WriteLine("[{0}] {1} Sais Bye Bye", DateTime.Now, userDeatails.UserName);
            }
            catch 
            {
                Console.WriteLine("Somthing is wrong with resourses close");            
            }
        }

        // Handles all the information from client.
        void InformationHandler()  
        {
            Console.WriteLine("[{0}] {1} is logged-in", DateTime.Now, userDeatails.UserName);
            userDeatails.isLoggedIn = true;
            try
            {   
                // Users are connected.
                while (tcpClient.Client.Connected)  
                {
                    // Get the information from client.
                    byte type = binaryReader.ReadByte(); 

                    // If user is available.
                    if (type == USER_AVAILABILITY_CHECK)
                    {

                        // Reading the user name to connect to.
                        string userToConnect = binaryReader.ReadString();
                        
                        // Response that every thing is ok.
                        binaryWriter.Write(USER_AVAILABILITY_CHECK);
                        
                        // Connected to this user.
                        binaryWriter.Write(userToConnect);
                        UserData userData;

                        // If users exists in DB.
                        if (myServer.users.TryGetValue(userToConnect, out userData))
                        {   

                            // And User is looged-in.
                            if (userData.isLoggedIn)
                            {
                                // User is available.
                                binaryWriter.Write(true);
                            }
                            else
                            {
                                // User is not available (not logged-in).
                                binaryWriter.Write(false);  
                            }
                        }
                        else
                        {
                            // User is not available.
                            binaryWriter.Write(false); 
                        }
                            binaryWriter.Flush();
                    }
        
                    // Sent message to user.
                    else if (type == SEND_MESSAGE)
                    {
                        string toUserName = binaryReader.ReadString();
                        string messageToSend = binaryReader.ReadString();
                        UserData sendToUser;
                        if (myServer.users.TryGetValue(toUserName, out sendToUser))
                        {
                            if (sendToUser.isLoggedIn)
                            {
                                // Send signal that message is recived.
                                sendToUser.clientHandler.binaryWriter.Write(MESSAGE_RECIVED);
                                sendToUser.clientHandler.binaryWriter.Write(userDeatails.UserName);  // From
                                sendToUser.clientHandler.binaryWriter.Write(messageToSend);
                                sendToUser.clientHandler.binaryWriter.Flush();
                                Console.WriteLine("[{0}] Message sent from {1} to {2} ", DateTime.Now, userDeatails.UserName, sendToUser.UserName);
                            }
                        }
                    }
                }
            }
            catch (IOException) { }
            userDeatails.isLoggedIn = false;
            Console.WriteLine("[{0}] {1} User just logged out", DateTime.Now, userDeatails.UserName);
        }
    }
}