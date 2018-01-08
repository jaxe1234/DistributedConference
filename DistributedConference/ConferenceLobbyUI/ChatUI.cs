﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConferenceLobbyUI
{
    public partial class ChatUI : Form
    {
        public ChatUI()
        {
            InitializeComponent();
        }

        private void msgToSendBox_TextChanged(object sender, EventArgs e)
        {
            // TODO: We should probably sanitize input and limit size
        }

        private void sendButton_Click(object sender, EventArgs e)
        {

            var msg = msgToSendBox.Text;
            receivedMessagesBox.Items.Add(msg);
            msgToSendBox.Clear();
            receivedMessagesBox.TopIndex = receivedMessagesBox.Items.Count - 1;

        }

        private void receivedMessagesBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void msgToSendBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.Enter)
            {
                //Shift+Enter to insert newline
                msgToSendBox.Text.Insert(msgToSendBox.SelectionStart, Environment.NewLine);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                //Enter to send
                sendButton.PerformClick();
            }

        }
    }
}
