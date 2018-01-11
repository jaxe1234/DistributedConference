using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;
using LoginServer;
using dotSpace.Objects.Network;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for ConferenceListWindow.xaml
    /// </summary>
    public partial class ConferenceListWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<string> conferenceTuple { get; set; }
        public RemoteSpace ConferenceRequests;

        public event PropertyChangedEventHandler PropertyChanged;

        private string User;


       


        public ConferenceListWindow(string Username)
        {
            DataContext = this;
            Task.Factory.StartNew(() => init());
            User = Username;
            InitializeComponent();
            RefreshButton.Click += RefreshButton_Click;
            ConfList.MouseDoubleClick += ConfList_MouseDoubleClick;
            NewConferenceButton.Click += NewConferenceButton_Click;

            

        }

        private void NewConferenceButton_Click(object sender, RoutedEventArgs e)
        {
            CreateConferenceWindow NewConfWin = new CreateConferenceWindow(ConferenceRequests, User, this);
            NewConfWin.Show();
        }

        private void ConfList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            asyncmethodthatwecall(e);
        }

        private async void asyncmethodthatwecall(MouseButtonEventArgs e)
        {
            var IPconnect = await Task<string>.Factory.StartNew(()=> otherasyncmethod(e));

        }

        private string otherasyncmethod(MouseButtonEventArgs e)
        {
            var conferenceClicked = (TextBlock)e.OriginalSource;
            //ConferenceRequests.Put(User, conferenceClicked.Text);
            var ip = (string)ConferenceRequests.Get(User, typeof(string))[1];
            return ip;
        }

        private void RefreshButton_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => conferenceTuple = new ObservableCollection<string>((List<string>)ConferenceRequests.Query(typeof(List<string>))[0]));
        }

        public void init()
        {
            ConferenceRequests = new RemoteSpace("tcp://10.16.169.224:5001/getConferenceList?CONN");
            conferenceTuple = new ObservableCollection<string>((List<string>)ConferenceRequests.Query(typeof(List<string>))[0]);
            //conferenceTuple = (ObservableCollection < string >) ConferenceRequests.Query(typeof(List<string>))[0];
        }












    }









}
