using dotSpace.Objects.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for CreateConferenceWindow.xaml
    /// </summary>
    public partial class CreateConferenceWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        RemoteSpace ConferenceRequests;
        string Username;
        ConferenceListWindow conferenceListWindow;

        public CreateConferenceWindow(RemoteSpace ConferenceRequests, string Username, ConferenceListWindow conferenceListWindow)
        {

            DataContext = this;
            this.conferenceListWindow = conferenceListWindow;
            this.ConferenceRequests = ConferenceRequests;
            this.Username = Username;
            InitializeComponent();
            NewConferenceName.KeyUp += EnterPressed;
            NewConferenceName.KeyDown += EnterPressed;
            CreateButton.Click += CreateButton_Click;

        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string newConferenceName = new TextRange(NewConferenceName.Document.ContentStart,NewConferenceName.Document.ContentEnd).Text.Trim();
            if (!string.IsNullOrWhiteSpace(newConferenceName))
            {
                var ipAddress = Dns.GetHostEntry("").AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                ConferenceRequests.Put(Username, newConferenceName, ipAddress, 1);
                ConferenceWindow conference = new ConferenceWindow(Username, newConferenceName);
                App.Current.MainWindow = conference;
                conferenceListWindow.Close();
                this.Close();
                conference.Show();
            }

        }

        private void EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CreateButton_Click(sender, null);

            }
        }
    }
}
