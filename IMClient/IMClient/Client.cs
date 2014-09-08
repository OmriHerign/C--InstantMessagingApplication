using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;

namespace IMClient
{
    public class Client
    {   
        // TCP Connection Thread
        Thread connectionThread;
      
        // Keep Aslive Flag
        bool isConnected = false;
        
        // Is user looged in already.
        bool isLoggedIn = false;  
        
        // Client User Name
        string userName;          

        // Client Password
        string userPassword;          
        
        // Is User registered.
        bool isOnRegMode;              

        // client in Tcp connection.
        TcpClient client;
        NetworkStream netStream;
        BinaryReader binaryReader;
        BinaryWriter binaryWriter;

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
        public const byte REQUEST_FOR_USER = 9;

        // Address of server. In this case - local IP address.
        public string Server
        {
            get
            {
                return "localhost";
            }
        }

        // Servers Port Getter
        public int Port { get { return 6000; } }

        // Is logged in getter.
        public bool getIsloggedIn
        {
            get
            {
                return this.isLoggedIn;
            }
        }
        public string UserName { 
            get { 
                return userName;
            }
        }
        public string Password {
            get {
                return userPassword;
            }
        }

        // Start connection thread and login or register.
        void UserConnection(string user, string password, bool register)
        {
            if (!isConnected)
            {
                isConnected = true;
                userName = user;
                userPassword = password;
                isOnRegMode = register;
                connectionThread = new Thread(new ThreadStart(SetupConnection));
                connectionThread.Start();
            }
        }

        // Login into the chat.
        public void ChatLogin(string user, string password)
        {
            UserConnection(user, password, false);
        }

        // Regiter in server.
        public void ChatRegisteration(string user, string password)
        {
            UserConnection(user, password, true);
        }

        // Disconnect from chat
        public void DisconnectFromChat()
        {
            if (isConnected){
                CloseClientConnection();
            }
        }
        // Is user Available.
        public void IsUserAvailable(string user)
        {
            // If user is connected to chat.
            if (isConnected)
            {
                binaryWriter.Write(USER_AVAILABILITY_CHECK);
                binaryWriter.Write(user);
                binaryWriter.Flush();
            }
        }

        // Send message to user.
        public void SendMessage(string to, string msg)
        {
            if (isConnected)
            {
                binaryWriter.Write(SEND_MESSAGE);
                binaryWriter.Write(to);
                binaryWriter.Write(msg);
                binaryWriter.Flush();
            }
        }
      
        //  Creating Events for user handling.
        public event EventHandler LoginOK;
        public event EventHandler RegisterOK;
        public event EventHandler Disconnected;
        public event ClientErrorEventHandler LoginFailed;
        public event ClientErrorEventHandler RegisterFailed;
        public event ClientAvailableEventsHandler UserAvailable;
        public event ClientReceivedMessageEventHandler MessageReceived;

        // If login succeded.
        virtual protected void LoggedInEvent()
        {
            if (LoginOK != null)
                LoginOK(this, EventArgs.Empty);
        }

        // Login Faild
        virtual protected void LoginFailedEvent(ClientErrorEvents e)
        {
            if (LoginFailed != null)
                LoginFailed(this, e);
        }

        // If registration Succeded.
        virtual protected void RegisteredEvent()
        {
            if (RegisterOK != null)
                RegisterOK(this, EventArgs.Empty);
        }

        // If registration failed
        virtual protected void RegisterationFailedEvent(ClientErrorEvents e)
        {
            if (RegisterFailed != null)
                RegisterFailed(this, e);
        }

        // When disconnected.
        virtual protected void DisconnectedEvent()
        {
            if (Disconnected != null)
                Disconnected(this, EventArgs.Empty);
        }

        // When User available.
        virtual protected void UserAvailableEvent(ClientAvailableEvent e)
        {
            if (UserAvailable != null)
                UserAvailable(this, e);
        }

        // When message recived.
        virtual protected void MessageRecivedEvent(ClientReceivedMessagesEvents e)
        {
            if (MessageReceived != null)
                MessageReceived(this, e);
        }
        
        // Start Connection and Handles the login.
        void SetupConnection()
        {
            try
            {
                // Connect to TCP server in specific port.
                client = new TcpClient(Server, Port);

                // NetworkStream used to send and receive data.
                netStream = client.GetStream();

                // Start new binary Reader and Writer for messages handle.
                binaryReader = new BinaryReader(netStream, Encoding.UTF8);
                binaryWriter = new BinaryWriter(netStream, Encoding.UTF8);

                // Receive "hello"
                int connectionEstablishSign = binaryReader.ReadInt32();
                if (connectionEstablishSign == ESTABLISH_CONNECTION)
                {
                    // Connection established.
                    binaryWriter.Write(ESTABLISH_CONNECTION);

                    // If user is either logged in or register.
                    binaryWriter.Write(isOnRegMode ? REGISTER_MODE : LOGIN_MODE);
                    binaryWriter.Write(UserName);
                    binaryWriter.Write(Password);
                    binaryWriter.Flush();

                    // Reads the next byte from the current stream.
                    byte serverAnswer = binaryReader.ReadByte();
                    if (serverAnswer == CONNECTED)
                    {
                        if (isOnRegMode)
                        {
                            RegisteredEvent();
                        }
                        LoggedInEvent();
                        DataHandler();
                    }
                    else
                    {
                        ClientErrorEvents error = new ClientErrorEvents((ClientErrorsEnum)serverAnswer);
                        if (isOnRegMode)
                        {
                            RegisterationFailedEvent(error);
                        }
                        else
                        {
                            LoginFailedEvent(error);
                        }
                    }
                }
                if (isConnected)
                {
                    CloseClientConnection();
                }
            }
            catch (Exception e)
            {
               
            }
        }
        
        // Closing all client resourses. 
        void CloseClientConnection() 
        {
            binaryReader.Close();
            binaryWriter.Close();
            netStream.Close();
            client.Close();
            DisconnectedEvent();
            isConnected = false;
        }

        // Handles the data from server.
        void DataHandler()  
        {
            this.isLoggedIn = true;
            try
            {
                // Ass long as the user is connected.
                while (client.Connected)                  {

                    // Read information from stream.
                    byte informationType = binaryReader.ReadByte();

                    if (informationType == USER_AVAILABILITY_CHECK)
                    {
                        string recivedFromUserName = binaryReader.ReadString();
                        bool isUserAvailable = binaryReader.ReadBoolean();
                        UserAvailableEvent(new ClientAvailableEvent(recivedFromUserName, isUserAvailable));
                    }
                    else if (informationType == MESSAGE_RECIVED)
                    {
                        string fromUser = binaryReader.ReadString();
                        string message = binaryReader.ReadString();
                        MessageRecivedEvent(new ClientReceivedMessagesEvents(fromUser, message));
                    }
                }
            }
            catch (IOException) { }
            this.isLoggedIn = false;
        }
    }
}