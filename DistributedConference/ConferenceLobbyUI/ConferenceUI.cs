using System;
using System.Windows.Forms;

namespace ConferenceLobbyUI
{
    public partial class ConferenceUI : Form
    {
        
        public ConferenceUI()
        {
            InitializeComponent();
        }

        private void ChatUI_Load(object sender, EventArgs e)
        {
        }

        private void msgToSendBox_TextChanged(object sender, EventArgs e)
        {
            // TODO: We should probably sanitize input and limit size
        }

        private void sendButton_Click(object sender, EventArgs e)
        {

            var msg = msgToSendBox.Text;
            receivedMessagesBox.AppendText(msg);
            msgToSendBox.Clear();
            //receivedMessagesBox

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

        private void receivedMessagesBox_TextChanged(object sender, EventArgs e)
        {
            receivedMessagesBox.SelectionStart = receivedMessagesBox.TextLength;
            receivedMessagesBox.ScrollToCaret();
        }
    }
}
