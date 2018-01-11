using dotSpace.Objects.Network;
using System;
using System.Collections.Generic;
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
        ConferenceListWindow oldWin;



        public CreateConferenceWindow(RemoteSpace ConferenceRequests, string Username, ConferenceListWindow oldWindow)
        {

            DataContext = this;
            oldWin = oldWindow;
            this.ConferenceRequests = ConferenceRequests;
            this.Username = Username;
            InitializeComponent();
            NewConferenceName.KeyUp += EnterPressed;
            NewConferenceName.KeyDown += EnterPressed;
            CreateButton.Click += CreateButton_Click;
            //LAV NULL CHEKCS FOR CONF NAME



        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(NewConferenceName.Document.ContentStart,NewConferenceName.Document.ContentEnd).Text.Trim();
            ConferenceRequests.Put(Username, text, 1);
            ConferenceWindow Conference = new ConferenceWindow(Username);
            App.Current.MainWindow = Conference;
            oldWin.Close();
            this.Close();
            Conference.Show();

        }

        private void EnterPressed(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                ConferenceRequests.Put(
                    Username,
                    new TextRange(
                        NewConferenceName.Document.ContentStart,
                        NewConferenceName.Document.ContentEnd
                    ).Text,
                    1
                );
                ConferenceWindow Conference = new ConferenceWindow(Username);
                App.Current.MainWindow = Conference;
                this.Close();
                Conference.Show();
                
               
            }
        }
    }
}
