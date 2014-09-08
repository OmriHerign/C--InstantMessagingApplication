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

    // Registration Form presentation.
    public partial class RegisterForm : Form
    {
        public string userName;
        public string password;

        public RegisterForm()
        {
            InitializeComponent();
        }

        // On button login click.
        private void LoginRegister_Click(object sender, EventArgs e)
        {
            userName = textBoxUserName.Text;
            password = textBoxPassword.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        // On cancel button click.
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        // On form load
        private void RegisterForm_Load(object sender, EventArgs e)
        {   
            // Size of form cannot change.
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }
    }
}