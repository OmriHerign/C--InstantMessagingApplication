namespace IMClient
{
    partial class SessionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonSend = new System.Windows.Forms.Button();
            this.sendMessageBox = new System.Windows.Forms.TextBox();
            this.coverstaionTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(458, 252);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(104, 23);
            this.buttonSend.TabIndex = 0;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // sendMessageBox
            // 
            this.sendMessageBox.Location = new System.Drawing.Point(12, 254);
            this.sendMessageBox.Name = "sendMessageBox";
            this.sendMessageBox.Size = new System.Drawing.Size(440, 20);
            this.sendMessageBox.TabIndex = 1;
            this.sendMessageBox.Text = "Write Your MessageHere ...";
            // 
            // coverstaionTextBox
            // 
            this.coverstaionTextBox.Location = new System.Drawing.Point(12, 6);
            this.coverstaionTextBox.Name = "coverstaionTextBox";
            this.coverstaionTextBox.Size = new System.Drawing.Size(550, 242);
            this.coverstaionTextBox.TabIndex = 2;
            this.coverstaionTextBox.Text = "";
            this.coverstaionTextBox.TextChanged += new System.EventHandler(this.coverstaionTextBox_TextChanged);
            // 
            // SessionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 287);
            this.Controls.Add(this.coverstaionTextBox);
            this.Controls.Add(this.sendMessageBox);
            this.Controls.Add(this.buttonSend);
            this.Location = new System.Drawing.Point(100, 100);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SessionForm";
            this.Load += new System.EventHandler(this.SessionForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox sendMessageBox;
        private System.Windows.Forms.RichTextBox coverstaionTextBox;
    }
}