using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace IMClient
{   
    // The conversation form between users.
    public partial class SessionForm : Form
    {   


        public Client client;
        public string toUser;
        ClientAvailableEventsHandler availabiltyHandler;
        ClientReceivedMessageEventHandler infoReceivedHandler;
        string usersDataFile;
        String conversation;
        StreamWriter file;

        public SessionForm(Client client, string userName)
        {
            try
            {
                InitializeComponent();
                this.client = client;
                this.toUser = userName;
                sendMessageBox.ForeColor = Color.LightGray;
                sendMessageBox.Text = "Please Enter Your Message";
                this.sendMessageBox.Leave += new System.EventHandler(this.sendMessageBox_Leave);
                this.sendMessageBox.Enter += new System.EventHandler(this.sendMessageBox_Enter);
                
                // If logs directory is bigger then 10MB.
                if (GetDirectorySize(Environment.CurrentDirectory + "\\Logs") > 10000000)
                {   
                    // Delete the oldest file.
                    GetOldestFile(Environment.CurrentDirectory + "\\Logs").Delete();
                }

                // Current time for the log file tag
                string dateAndTime = DateTime.Now.ToString("dd-MM-yyyy");
                
                // Log file path.
                usersDataFile = Environment.CurrentDirectory + "\\Logs\\[" + dateAndTime + "]_Conversation_Log_" + this.client.UserName + "_to_" + userName + ".txt";
                
                // Create Log File if exists append to the end. 
                file = new StreamWriter(usersDataFile, true);
            }
            catch
            {
                MessageBox.Show("Somthing Wrong with Loading Form");
            }
        }

        private void sendMessageBox_Leave(object sender, EventArgs e)
        {
            if (sendMessageBox.Text.Length == 0)
            {
                sendMessageBox.Text = "Please Enter Your Message";
                sendMessageBox.ForeColor = Color.LightGray;
            }
        }
        private void sendMessageBox_Enter(object sender, EventArgs e)
        {
            if (sendMessageBox.Text == "Please Enter Your Message")
            {
                sendMessageBox.Text = "";
                sendMessageBox.ForeColor = Color.Black;
            }
        }

        // On form loading
        private void SessionForm_Load(object sender, EventArgs e)
        {   
            // Form title.
            this.Text = "Conversation with "+ toUser;
            availabiltyHandler = new ClientAvailableEventsHandler(OnUserAvailabilty);
            infoReceivedHandler = new ClientReceivedMessageEventHandler(ReciveMessage);
            client.UserAvailable += availabiltyHandler;
            client.MessageReceived += infoReceivedHandler;
            client.IsUserAvailable(toUser);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

        }

        // On closing Form.
        private void SessionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.UserAvailable -= availabiltyHandler;
            client.MessageReceived -= infoReceivedHandler;
        }

        // When clicking on the send button.
        private void buttonSend_Click(object sender, EventArgs e)
        {
            client.SendMessage(toUser, sendMessageBox.Text);
            coverstaionTextBox.Text += String.Format("[{0}] {1}\r\n", client.UserName, sendMessageBox.Text);
            conversation += String.Format("[{0}] {1}\r\n", client.UserName, sendMessageBox.Text);
            sendMessageBox.Text = "";
        }
        
        bool isUserAvailable = false;

        // When user is available.
        void OnUserAvailabilty(object sender, ClientAvailableEvent e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                if (e.UserName == toUser)
                {
                    if (isUserAvailable != e.IsAvailable)
                    {
                        isUserAvailable = e.IsAvailable;
                        string avail = (e.IsAvailable ? "available" : "unavailable");
                        this.Text = String.Format("{0} - {1}", toUser, avail);
                        coverstaionTextBox.Text += String.Format("[{0} is {1}]\r\n", toUser, avail);
                    }
                }
            }));
        }

        // Recive Message from another user.
        void ReciveMessage(object sender, ClientReceivedMessagesEvents e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                if (e.FromUserName == toUser)
                {
                    coverstaionTextBox.Text += String.Format("[{0}] {1}\r\n", e.FromUserName, e.MessageRecived);
                    conversation += String.Format("[{0}] {1}\r\n", e.FromUserName, e.MessageRecived);
                }
            }));
        }

        private void coverstaionTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        // On clicking the "x" button.
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            disconnectAndSave();
        }

        // User disconnect and conversation save.
        void disconnectAndSave()
        {   
            // Write conversation into the file.
            file.WriteLine(conversation);
            
            // Sign to the other user that the user logged out from the system.
            client.SendMessage(toUser, client.UserName + " LoggedOut");
            
            // Disconnecting user.
            this.client.DisconnectFromChat();
            
            // Flush into the file.
            file.Flush();
            
            // Close the file resource.
            file.Close();
        }
        
        // Finds directory size by bits. 
        long GetDirectorySize(string parentDirectory)
        {   
            // Sums file size in dir.
            return new DirectoryInfo(parentDirectory).GetFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);
        }

        // Finds the oldest file in the spesific directory.
        FileInfo GetOldestFile(string directory)
        {

            // Checks if directory exists.
            if (!Directory.Exists(directory))
                throw new ArgumentException();

            DirectoryInfo parent = new DirectoryInfo(directory);
            FileInfo[] children = parent.GetFiles();
            if (children.Length == 0)
                return null;

            FileInfo oldest = children[0];
            
            // BubbleSort by date over the files directory.
            foreach (var child in children.Skip(1))
            {
                if (child.CreationTime < oldest.CreationTime)
                    oldest = child;
            }

            // Returns older file in the directory.
            return oldest;
        }
    }
}