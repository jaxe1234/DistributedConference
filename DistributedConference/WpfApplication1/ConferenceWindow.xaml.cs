
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        private string username;
        private string Password;
        public string ConferenceName { get; set; }


        public ConferenceWindow(string username, string conferenceName, string Password) //For host
        {
            
            DataContext = this;
            this.Password = Password;
            this.username = username;
            this.ConferenceName = conferenceName;
            InitializeComponent();
            this.SizeChanged += Resize;
            SendButton.Click += SendButton_Click;
            this.TxtToSend = new TextRange(SendField.Document.ContentStart, SendField.Document.ContentEnd);
            SendField.KeyUp += SendField_KeyUp;
            this.MsgList = new ObservableCollection<string>();
            this.Loaded += MainWindow_Loaded;
            SpaceRepository spaceRepository = new SpaceRepository();
            this.conference = new ConferenceInitializer(username, conferenceName, MsgList, spaceRepository);
            
        }

        public ConferenceWindow(string username, string conferenceName, string ip, string Password) //For client
        {
            DataContext = this;
            this.Password = Password;
            this.username = username;
            this.ConferenceName = conferenceName;
            InitializeComponent();
            this.SizeChanged += Resize;
            SendButton.Click += SendButton_Click;
            this.TxtToSend = new TextRange(SendField.Document.ContentStart, SendField.Document.ContentEnd);
            SendField.KeyUp += SendField_KeyUp;
            this.MsgList = new ObservableCollection<string>();
            this.Loaded += MainWindow_Loaded;
            this.conference = new ConferenceInitializer(username, conferenceName, ip, MsgList);
            MsgList.CollectionChanged += NewMessageReceived;
        }

        private void NewMessageReceived(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(VisualTreeHelper.GetChildrenCount(ChatView)>0)
            {
                var b = VisualTreeHelper.GetChild(ChatView, 0);
                var s = (ScrollViewer)VisualTreeHelper.GetChild(b, 0);
                s.ScrollToBottom();
            }
           //((ListView)sender).ScrollIntoView(e.NewItems[e.NewItems.Count - 1]);
            //var selectedIndex = ChatView.Items.Count - 1;
            //if (selectedIndex < 0) return;
            //ChatView.SelectedIndex = selectedIndex;
            //ChatView.UpdateLayout();
            //ChatView.ScrollIntoView(ChatView.SelectedItem);
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
