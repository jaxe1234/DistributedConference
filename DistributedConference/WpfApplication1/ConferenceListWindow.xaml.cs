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

using dotSpace.Objects.Network;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for ConferenceListWindow.xaml
    /// </summary>
    public partial class ConferenceListWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<string> conferenceTuple { get; set; }
        public RemoteSpace ConferenceRequests;
        public RemoteSpace LoginSpace;

        public event PropertyChangedEventHandler PropertyChanged;

        private string Username;
        private string Password;
        private string PubKey;

        //*****************************************************************************************//
        //from https://stackoverflow.com/questions/743906/how-to-hide-close-button-in-wpf-window   //
        private const int GWL_STYLE = -16;                                                         //
        private const int WS_SYSMENU = 0x80000;                                                    //
        [DllImport("user32.dll", SetLastError = true)]                                             //
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);                          //
        [DllImport("user32.dll")]                                                                  //
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);           //
        //from https://stackoverflow.com/questions/743906/how-to-hide-close-button-in-wpf-window   //
        //*****************************************************************************************//

        public ConferenceListWindow(string Username, string Password, string PubKey, RemoteSpace LoginSpace)
        {
            DataContext = this;
            Task.Factory.StartNew(Init);
            this.Username = Username;
            this.Password = Password;
            this.PubKey = PubKey;
            this.LoginSpace = LoginSpace;
            InitializeComponent();
            RefreshButton.Click += RefreshButton_Click;
            //ConfList.MouseDoubleClick += ConfList_MouseDoubleClick;
            NewConferenceButton.Click += NewConferenceButton_Click;
            this.Loaded += Hack;

            

        }

        private void Hack(object sender, RoutedEventArgs e) 
        {
            //*****************************************************************************************//
            //from https://stackoverflow.com/questions/743906/how-to-hide-close-button-in-wpf-window   //
            var hwnd = new WindowInteropHelper(this).Handle;                                           //
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);              //                                                                                                         
            //from https://stackoverflow.com/questions/743906/how-to-hide-close-button-in-wpf-window   //
            //*****************************************************************************************//

        }



        private void NewConferenceButton_Click(object sender, RoutedEventArgs e)
        {
            
            CreateConferenceWindow NewConfWin = new CreateConferenceWindow(ConferenceRequests, Username, this, Password, LoginSpace);
            NewConfWin.Show();
        }

        private void ConfList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string conferenceClicked =((Button)sender).Content as string;
            ConnectToConference(conferenceClicked);
        }

        private async void ConnectToConference(string conferenceClicked)
        {
            var IPconnect = await Task<string>.Factory.StartNew(()=> GetIpFromServer(conferenceClicked));
            if (!IPconnect.Equals(""))
            {
                ConferenceWindow conference = new ConferenceWindow(Username, conferenceClicked, IPconnect, Password, LoginSpace);
                App.Current.MainWindow = conference;
                this.Close();
                conference.Show();
            }
            else
            {
                MessageBox.Show("Server rejected request. Try again, or log out and back in.", "Server fault", MessageBoxButton.OK, MessageBoxImage.Information);

            }

        }

        private string GetIpFromServer(string conferenceClicked)
        {
            
            ConferenceRequests.Put(Username, conferenceClicked, 0, RSA.RSAEncrypt(Password));
            var temp = ConferenceRequests.Get(Username, typeof(string), typeof(int));
            var ip = (string)temp[1];
            return ip;
        }

        private void RefreshButton_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => conferenceTuple = new ObservableCollection<string>((List<string>)ConferenceRequests.Query(typeof(List<string>))[0]));
        }

        public void Init()
        {
            ConferenceRequests = new RemoteSpace("tcp://" + _Resources.Resources.InternetProtocolAddress +":5001/getConferenceList?CONN");
            conferenceTuple = new ObservableCollection<string>((List<string>)ConferenceRequests.Query(typeof(List<string>))[0]);
            //conferenceTuple = (ObservableCollection < string >) ConferenceRequests.Query(typeof(List<string>))[0];
        }












    }









}
