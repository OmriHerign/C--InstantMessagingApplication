using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IMClient
{

    // This class presents the form login.
    public partial class LoginForm : Form
    {
        Client client = new Client();
        
        // Save list of seesions.
        public List<SessionForm> sessions = new List<SessionForm>();
        
        // On creating ....
        public LoginForm()
        {
            InitializeComponent();
            
            // Disable unavailabe buttons.
            buttonConnect.Enabled = false;
            buttonAddUser.Enabled = false;
            buttonLogout.Enabled = false;
            
            // Start events handlers.
            client.LoginOK += new EventHandler(ClientLoginEvent);
            client.RegisterOK += new EventHandler(RegistrationOk);
            client.LoginFailed += new ClientErrorEventHandler(FailedToLogin);
            client.RegisterFailed += new ClientErrorEventHandler(FailedToRegister);
            client.Disconnected += new EventHandler(UserDisconnected);
        }

        // On form loading
        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Size of Form does not changed.
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        // On register button click.
        private void buttonRegister_Click(object sender, EventArgs e)
        {   
            // New registartion form.
            RegisterForm newUser = new RegisterForm();
            if (newUser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.client.ChatRegisteration(newUser.userName, newUser.password);
            }
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {   
            // Login return user.
            RegisterForm oldUser = new RegisterForm();
            if (oldUser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.client.ChatLogin(oldUser.userName, oldUser.password);
            }
        }

        // Logout from chat.
        private void buttonLogout_Click(object sender, EventArgs e)
        {
            this.client.DisconnectFromChat();
        }
       
        // Connect to chat.
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            SessionForm conversation = new SessionForm(client, loggedInUsers.SelectedItem.ToString());
            addUserText.Text = "";
            sessions.Add(conversation);
            conversation.Show();
        }

        // On Login.
        void ClientLoginEvent(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                // Enable and Disable the right buttons.
                buttonRegister.Enabled = false;
                buttonLogin.Enabled = false;
                buttonLogout.Enabled = true;
                buttonAddUser.Enabled = true;
            }));
        }

        // Registration phase passed.
        void RegistrationOk(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {

                // Enable and Disable the right buttons.
                buttonRegister.Enabled = false;
                buttonLogin.Enabled = false;
                buttonLogout.Enabled = true;
                buttonAddUser.Enabled = true;
            }));
        }

        // Cannot log-in.
        void FailedToLogin(object sender, ClientErrorEvents e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                MessageBox.Show("Login failed");
            }));
        }

        // Cannot register.
        void FailedToRegister(object sender, ClientErrorEvents e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                MessageBox.Show("Registeration failed");
            }));
        }

        // The user just dissconnect.
        void UserDisconnected(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {

                // Enable and Disable the right buttons.
                buttonRegister.Enabled = true;
                buttonLogin.Enabled = true;
                buttonLogout.Enabled = false;
                
                // Close All sessions.
                foreach (SessionForm conversation in sessions)
                {
                    conversation.Close();
                    loggedInUsers.Items.Remove(conversation.client.UserName);
                }
            }));
        }

        // When click on the Add button
        private void buttonAddUser_Click(object sender, EventArgs e)
        {
            loggedInUsers.Items.Add(addUserText.Text);
            addUserText.Text = "";
        }

        // When select user from the list.
        private void loggedInUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonConnect.Enabled = true;
        }
    }
}
