namespace ConferenceLobbyUI
{
    partial class ConferenceUI
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
            this.receivedMessagesBox = new System.Windows.Forms.RichTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.findLobbyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backButton = new System.Windows.Forms.Button();
            this.jumpToLiveButton = new System.Windows.Forms.Button();
            this.forwarButton = new System.Windows.Forms.Button();
            this.jumpToPageButton = new System.Windows.Forms.Button();
            this.jumpToPageBox = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // msgToSendBox
            // 
            this.msgToSendBox.Location = new System.Drawing.Point(692, 481);
            this.msgToSendBox.Name = "msgToSendBox";
            this.msgToSendBox.Size = new System.Drawing.Size(134, 53);
            this.msgToSendBox.TabIndex = 0;
            this.msgToSendBox.Text = "";
            this.msgToSendBox.TextChanged += new System.EventHandler(this.msgToSendBox_TextChanged);
            this.msgToSendBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.msgToSendBox_KeyDown);
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(832, 481);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(88, 53);
            this.sendButton.TabIndex = 1;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // receivedMessagesBox
            // 
            this.receivedMessagesBox.Location = new System.Drawing.Point(691, 27);
            this.receivedMessagesBox.Name = "receivedMessagesBox";
            this.receivedMessagesBox.ReadOnly = true;
            this.receivedMessagesBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.receivedMessagesBox.Size = new System.Drawing.Size(224, 448);
            this.receivedMessagesBox.TabIndex = 2;
            this.receivedMessagesBox.Text = "";
            this.receivedMessagesBox.TextChanged += new System.EventHandler(this.receivedMessagesBox_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(674, 507);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findLobbyToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(927, 28);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // findLobbyToolStripMenuItem
            // 
            this.findLobbyToolStripMenuItem.Name = "findLobbyToolStripMenuItem";
            this.findLobbyToolStripMenuItem.Size = new System.Drawing.Size(91, 24);
            this.findLobbyToolStripMenuItem.Text = "Find lobby";
            // 
            // backButton
            // 
            this.backButton.Location = new System.Drawing.Point(12, 547);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(75, 32);
            this.backButton.TabIndex = 5;
            this.backButton.Text = "Back";
            this.backButton.UseVisualStyleBackColor = true;
            // 
            // jumpToLiveButton
            // 
            this.jumpToLiveButton.Location = new System.Drawing.Point(93, 547);
            this.jumpToLiveButton.Name = "jumpToLiveButton";
            this.jumpToLiveButton.Size = new System.Drawing.Size(108, 32);
            this.jumpToLiveButton.TabIndex = 6;
            this.jumpToLiveButton.Text = "Jump to live";
            this.jumpToLiveButton.UseVisualStyleBackColor = true;
            // 
            // forwarButton
            // 
            this.forwarButton.Location = new System.Drawing.Point(402, 547);
            this.forwarButton.Name = "forwarButton";
            this.forwarButton.Size = new System.Drawing.Size(75, 32);
            this.forwarButton.TabIndex = 7;
            this.forwarButton.Text = "Forward";
            this.forwarButton.UseVisualStyleBackColor = true;
            // 
            // jumpToPageButton
            // 
            this.jumpToPageButton.Location = new System.Drawing.Point(208, 547);
            this.jumpToPageButton.Name = "jumpToPageButton";
            this.jumpToPageButton.Size = new System.Drawing.Size(113, 32);
            this.jumpToPageButton.TabIndex = 8;
            this.jumpToPageButton.Text = "Jump to page:";
            this.jumpToPageButton.UseVisualStyleBackColor = true;
            // 
            // jumpToPageBox
            // 
            this.jumpToPageBox.Location = new System.Drawing.Point(327, 547);
            this.jumpToPageBox.Name = "jumpToPageBox";
            this.jumpToPageBox.Size = new System.Drawing.Size(69, 32);
            this.jumpToPageBox.TabIndex = 9;
            this.jumpToPageBox.Text = "";
            // 
            // ConferenceUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(927, 591);
            this.Controls.Add(this.jumpToPageBox);
            this.Controls.Add(this.jumpToPageButton);
            this.Controls.Add(this.forwarButton);
            this.Controls.Add(this.jumpToLiveButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.receivedMessagesBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.msgToSendBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ConferenceUI";
            this.Text = "Conference";
            this.Load += new System.EventHandler(this.ChatUI_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox msgToSendBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.RichTextBox receivedMessagesBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem findLobbyToolStripMenuItem;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Button jumpToLiveButton;
        private System.Windows.Forms.Button forwarButton;
        private System.Windows.Forms.Button jumpToPageButton;
        private System.Windows.Forms.RichTextBox jumpToPageBox;
    }
}

