
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using SlideCommunication;
using System.IO;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for ConferenceWindow.xaml
    /// </summary>
    public partial class ConferenceWindow : Window, INotifyPropertyChanged, ISlideShow
    {

        public int SndBttnHeight { get; set; }
        public int ChtFldHeight { get; set; }
        public ObservableCollection<string> MsgList { get; set; }
        TextRange TxtToSend;
        private ConferenceInitializer conference;
        private string username;
        private string Password;
        public string ConferenceName { get; set; }
        private RemoteSpace ConferenceRequests { get; set; }
        public bool IsHost { get; set; }
        public bool InControl { get; set; }
        public int? CurrentPage { get; private set; }
        public int? NumberOfPages { get; private set; }
        Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
        public BitmapImage ImageToShow { get; set; }


        public ConferenceWindow(string username, string conferenceName, string password, RemoteSpace conferenceRequests) //For host
        {

            DataContext = this;
            this.Password = password;
            this.username = username;
            this.ConferenceName = conferenceName;
            dlg.FileName = "Presentation"; // Default file name
            dlg.DefaultExt = ".pdf"; // Default file extension
            dlg.Filter = "PDF documents (.pdf)|*.pdf"; // Filter files by extension

            InitializeComponent();

            SpaceRepository spaceRepository = new SpaceRepository();
            this.TxtToSend = new TextRange(SendField.Document.ContentStart, SendField.Document.ContentEnd);
            this.MsgList = new ObservableCollection<string>();
            this.conference = new ConferenceInitializer(username, conferenceName, MsgList, spaceRepository, this);
            this.ConferenceRequests = conferenceRequests;

            this.Loaded                    += MainWindow_Loaded;
            Closed                         += OnClose_Host;
            SendField.KeyUp                += SendField_KeyUp;
            this.SizeChanged               += Resize;
            SendButton.Click               += SendButton_Click;
            GoBackwards.Click              += GoBackwards_Click;
            GoForwad.Click                 += GoForwad_Click;
            OpenPresentaion.MouseDown      += OpenPresentaion_Click;
            
        }

        private void OnClose_Host(object sender, EventArgs eventArgs)
        {
            var ip = NamingTools.IpFetcher.GetLocalIpAdress();
            ConferenceRequests.Put(username, ConferenceName, ip, 0, RSA.RSAEncrypt(Password));
            Environment.Exit(0);
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
            this.conference = new ConferenceInitializer(username, conferenceName, ip, MsgList, this);
            MsgList.CollectionChanged += NewMessageReceived;
            Closed += OnClose_Client;




        }

        private void OpenPresentaion_Click(object sender, RoutedEventArgs e)
        {
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                var stream = new FileStream(filename, FileMode.Open);
                conference.Host.PrepareToHost(stream);
                //CurrentPage = 0;
                //GoForwad_Click(null, null);
            }
        }

        private void GoForwad_Click(object sender, RoutedEventArgs e)
        {
            conference.Host.Control.PageNumber++;
        }

        private void GoBackwards_Click(object sender, RoutedEventArgs e)
        {
            conference.Host.Control.PageNumber--;
        }

        private void OnClose_Client(object sender, EventArgs e)
        {
            conference.ChatSender.SendMessage("Left the chat");
            Environment.Exit(0);
        }

        private void NewMessageReceived(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount(ChatView) > 0)
            {
                var b = VisualTreeHelper.GetChild(ChatView, 0);
                var s = (ScrollViewer)VisualTreeHelper.GetChild(b, 0);
                s.ScrollToBottom();
            }

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

        public void UpdateSlide(FramePayload payload)
        {
            CurrentPage = payload.PageNumber;

            Application.Current.Dispatcher.Invoke(() =>
            {
                using (var memestreem = new MemoryStream(payload.Bitstream))
                {

                    var _imageToShow = new BitmapImage();
                    _imageToShow.BeginInit();
                    _imageToShow.StreamSource = memestreem;//payload.Bitstream;
                    _imageToShow.CacheOption = BitmapCacheOption.OnLoad;
                    _imageToShow.EndInit();
                    ImageToShow = _imageToShow;
                }
            });
        }

        public void NewCollection(int pages)
        {
            NumberOfPages = pages;
        }

        // Not in use
        public void Draw(Shape figure, System.Drawing.Point position)
        {
            
            throw new NotImplementedException();
        }
    }
}
