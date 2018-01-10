
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public int SndBttnHeight { get; set; }
        public int ChtFldHeight { get; set; }
        public ObservableCollection<string> MsgList { get; set; }
        TextRange TxtToSend;


    public MainWindow()
        {
            //Chat CurrentChat = new Chat();
            MsgList = new ObservableCollection<string>();
            InitializeComponent();
            DataContext = this;
            this.Loaded += MainWindow_Loaded;
            this.SizeChanged += Resize;
            SendButton.Click += SendButton_Click;
            TxtToSend =  new TextRange(SendField.Document.ContentStart, SendField.Document.ContentEnd);
            SendField.KeyUp += SendField_KeyUp;
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
            if (!string.IsNullOrWhiteSpace(TxtToSend.Text))
            {

                MsgList.Add(TxtToSend.Text.Trim());
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
            if( ((int)(this.Height / 12) - 5) < 32)
            {
                SndBttnHeight = (int)(this.Height / 12) -5;
                ChtFldHeight = SndBttnHeight - 5;
            }
            


        }
    }
}
