
namespace View
{
    partial class View
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
            this.playerNameBox = new System.Windows.Forms.TextBox();
            this.serverAddressBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.playerNamelabel = new System.Windows.Forms.Label();
            this.serverAddressLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // playerNameBox
            // 
            this.playerNameBox.Location = new System.Drawing.Point(60, 25);
            this.playerNameBox.Name = "playerNameBox";
            this.playerNameBox.Size = new System.Drawing.Size(100, 20);
            this.playerNameBox.TabIndex = 0;
            // 
            // serverAddressBox
            // 
            this.serverAddressBox.Location = new System.Drawing.Point(166, 25);
            this.serverAddressBox.Name = "serverAddressBox";
            this.serverAddressBox.Size = new System.Drawing.Size(100, 20);
            this.serverAddressBox.TabIndex = 1;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(272, 22);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // playerNamelabel
            // 
            this.playerNamelabel.AutoSize = true;
            this.playerNamelabel.Location = new System.Drawing.Point(57, 9);
            this.playerNamelabel.Name = "playerNamelabel";
            this.playerNamelabel.Size = new System.Drawing.Size(67, 13);
            this.playerNamelabel.TabIndex = 3;
            this.playerNamelabel.Text = "Player Name";
            // 
            // serverAddressLabel
            // 
            this.serverAddressLabel.AutoSize = true;
            this.serverAddressLabel.Location = new System.Drawing.Point(163, 9);
            this.serverAddressLabel.Name = "serverAddressLabel";
            this.serverAddressLabel.Size = new System.Drawing.Size(79, 13);
            this.serverAddressLabel.TabIndex = 4;
            this.serverAddressLabel.Text = "Server Address";
            // 
            // View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 900);
            this.Controls.Add(this.serverAddressLabel);
            this.Controls.Add(this.playerNamelabel);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.serverAddressBox);
            this.Controls.Add(this.playerNameBox);
            this.Name = "View";
            this.Text = "PewPewPew";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox playerNameBox;
        private System.Windows.Forms.TextBox serverAddressBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label playerNamelabel;
        private System.Windows.Forms.Label serverAddressLabel;
    }
}

