﻿using System;
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

        private string Username;

        public ConferenceListWindow(string Username)
        {
            DataContext = this;
            Task.Factory.StartNew(() => init());
            this.Username = Username;
            InitializeComponent();
            RefreshButton.Click += RefreshButton_Click;
            ConfList.MouseDoubleClick += ConfList_MouseDoubleClick;
            NewConferenceButton.Click += NewConferenceButton_Click;

            

        }

        private void NewConferenceButton_Click(object sender, RoutedEventArgs e)
        {
            CreateConferenceWindow NewConfWin = new CreateConferenceWindow(ConferenceRequests, Username, this);
            NewConfWin.Show();
        }

        private void ConfList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var conferenceClicked = ((TextBlock)e.OriginalSource).Text;
            ConnectToConference(conferenceClicked);
        }

        private async void ConnectToConference(string conferenceClicked)
        {
            var IPconnect = await Task<string>.Factory.StartNew(()=> GetIpFromServer(conferenceClicked));

            ConferenceWindow conference = new ConferenceWindow(Username, conferenceClicked, IPconnect);
            App.Current.MainWindow = conference;
            this.Close();
            conference.Show();

        }

        private string GetIpFromServer(string conferenceClicked)
        {
            
            ConferenceRequests.Put(Username, conferenceClicked);
            var ip = (string)ConferenceRequests.Get(Username, typeof(string))[1];
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
