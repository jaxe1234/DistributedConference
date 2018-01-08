namespace ConferenceLobbyUI
{
    partial class TEST
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.conferenceNameTextBox = new System.Windows.Forms.TextBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ipTextBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.hostCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(159, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Conference name";
            // 
            // conferenceNameTextBox
            // 
            this.conferenceNameTextBox.Location = new System.Drawing.Point(162, 40);
            this.conferenceNameTextBox.Name = "conferenceNameTextBox";
            this.conferenceNameTextBox.Size = new System.Drawing.Size(108, 22);
            this.conferenceNameTextBox.TabIndex = 3;
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(16, 40);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(105, 22);
            this.nameTextBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "IP";
            // 
            // ipTextBox
            // 
            this.ipTextBox.Location = new System.Drawing.Point(16, 108);
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.Size = new System.Drawing.Size(105, 22);
            this.ipTextBox.TabIndex = 6;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(162, 106);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(108, 23);
            this.connectButton.TabIndex = 7;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // hostCheckBox
            // 
            this.hostCheckBox.AutoSize = true;
            this.hostCheckBox.Location = new System.Drawing.Point(16, 170);
            this.hostCheckBox.Name = "hostCheckBox";
            this.hostCheckBox.Size = new System.Drawing.Size(67, 21);
            this.hostCheckBox.TabIndex = 8;
            this.hostCheckBox.Text = "Host?";
            this.hostCheckBox.UseVisualStyleBackColor = true;
            // 
            // TEST
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.hostCheckBox);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.ipTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.conferenceNameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "TEST";
            this.Text = "Start";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox conferenceNameTextBox;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ipTextBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.CheckBox hostCheckBox;
    }
}