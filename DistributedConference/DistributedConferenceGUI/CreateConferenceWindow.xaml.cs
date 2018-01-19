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

namespace DistributedConferenceGUI
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
        string Password;
        private RemoteSpace LoginSpace;

        public CreateConferenceWindow(RemoteSpace ConferenceRequests, string Username, ConferenceListWindow conferenceListWindow, string Password, RemoteSpace LoginSpace)
        {

            DataContext = this;
            this.conferenceListWindow = conferenceListWindow;
            this.ConferenceRequests = ConferenceRequests;
            this.LoginSpace = LoginSpace;
            this.Username = Username;
            this.Password = Password;
            InitializeComponent();
            NewConferenceName.Focus();
            NewConferenceName.KeyUp += EnterPressed;
            NewConferenceName.KeyDown += EnterPressed;
            CreateButton.Click += CreateButton_Click;
            cancelButton.Click += CancelClick;

        }




        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
            DataContext = conferenceListWindow;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string newConferenceName = new TextRange(NewConferenceName.Document.ContentStart, NewConferenceName.Document.ContentEnd).Text.Trim();
            if (!string.IsNullOrWhiteSpace(newConferenceName))
            {
                var ipAddress = ProjectUtilities.IpFetcher.GetLocalIpAdress();
                ConferenceRequests.Put(Username, newConferenceName, ipAddress, 1, RSA.RSAEncrypt(Password));
                var feedback = ConferenceRequests.Get("Result", typeof(int), Username);
                if ((int)feedback[1] == 1)
                {
                    ConferenceWindow conference = new ConferenceWindow(Username, newConferenceName, Password, ConferenceRequests, LoginSpace);
                    App.Current.MainWindow = conference;
                    conferenceListWindow.Close();
                    this.Close();
                    conference.Show();
                }
                else
                {
                    MessageBox.Show("Server rejected request. Try again, or log out and back in.", "Server fault", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                    DataContext = conferenceListWindow;
                }

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
