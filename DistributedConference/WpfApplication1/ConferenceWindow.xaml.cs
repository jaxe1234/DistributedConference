
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Conference;
using dotSpace.Objects.Network;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for ConferenceWindow.xaml
    /// </summary>
    public partial class ConferenceWindow : Window, INotifyPropertyChanged
    {

        public int SndBttnHeight { get; set; }
        public int ChtFldHeight { get; set; }
        public ObservableCollection<string> MsgList { get; set; }
        TextRange TxtToSend;
        private ConferenceInitializer conference;


        public ConferenceWindow(string username, string conferenceName) //For host
        {
            SetUpConferenceWindow();
            SpaceRepository spaceRepository = new SpaceRepository();
            this.conference = new ConferenceInitializer(username, conferenceName, MsgList, spaceRepository);
            
        }

        public ConferenceWindow(string username, string conferenceName, string ip) //For client
        {
            SetUpConferenceWindow();
            this.conference = new ConferenceInitializer(username, conferenceName, ip, MsgList);
        }

        public void SetUpConferenceWindow()
        {
            InitializeComponent();
            DataContext = this;
            this.Loaded += MainWindow_Loaded;
            this.SizeChanged += Resize;
            SendButton.Click += SendButton_Click;
            TxtToSend = new TextRange(SendField.Document.ContentStart, SendField.Document.ContentEnd);
            SendField.KeyUp += SendField_KeyUp;
            this.MsgList = new ObservableCollection<string>();
        }

        private void SendField_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == (Key.Enter))
            {
                SendButton_Click(sender, null);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string textToSend = TxtToSend.Text.Trim();
            if (!string.IsNullOrWhiteSpace(textToSend))
            {

                conference.ChatSender.SendMessage(textToSend);
                SendField.Document.Blocks.Clear();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.MinWidth = this.Width;
            this.MinHeight = this.Height;
        }

        void Resize(object sender, RoutedEventArgs e)
        {
            if (((int)(this.Height / 12) - 5) < 32)
            {
                SndBttnHeight = (int)(this.Height / 12) - 5;
                ChtFldHeight = SndBttnHeight - 5;
            }



        }
    }
}
