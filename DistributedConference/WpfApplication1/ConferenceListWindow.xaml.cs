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




       


        public ConferenceListWindow()
        {
            Task.Factory.StartNew(() => init());
           

            DataContext = this;
           
           
            InitializeComponent();
            
           
        }

        public void init()
        {
            conferenceTuple = new ObservableCollection<string>();
            conferenceTuple = ConferenceRequests.Query(typeof(List<string>))[0] as ObservableCollection<string>;
        }












    }









}
