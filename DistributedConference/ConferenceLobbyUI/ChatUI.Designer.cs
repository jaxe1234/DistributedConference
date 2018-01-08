namespace ConferenceLobbyUI
{
    partial class ChatUI
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
            this.msgToSendBox = new System.Windows.Forms.RichTextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.receivedMessagesBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // msgToSendBox
            // 
            this.msgToSendBox.Location = new System.Drawing.Point(12, 390);
            this.msgToSendBox.Name = "msgToSendBox";
            this.msgToSendBox.Size = new System.Drawing.Size(334, 53);
            this.msgToSendBox.TabIndex = 0;
            this.msgToSendBox.Text = "";
            this.msgToSendBox.TextChanged += new System.EventHandler(this.msgToSendBox_TextChanged);
            this.msgToSendBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.msgToSendBox_KeyDown);
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(352, 390);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(88, 53);
            this.sendButton.TabIndex = 1;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // receivedMessagesBox
            // 
            this.receivedMessagesBox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.receivedMessagesBox.FormattingEnabled = true;
            this.receivedMessagesBox.ItemHeight = 24;
            this.receivedMessagesBox.Location = new System.Drawing.Point(12, 13);
            this.receivedMessagesBox.Name = "receivedMessagesBox";
            this.receivedMessagesBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.receivedMessagesBox.Size = new System.Drawing.Size(428, 364);
            this.receivedMessagesBox.TabIndex = 2;
            this.receivedMessagesBox.SelectedIndexChanged += new System.EventHandler(this.receivedMessagesBox_SelectedIndexChanged);
            // 
            // ChatUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 455);
            this.Controls.Add(this.receivedMessagesBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.msgToSendBox);
            this.Name = "ChatUI";
            this.Text = "Chat";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox msgToSendBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.ListBox receivedMessagesBox;
    }
}

