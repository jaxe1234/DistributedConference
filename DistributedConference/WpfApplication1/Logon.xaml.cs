using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for ConferenceListWindow.xaml
    /// </summary>
    public partial class LogonWindow : Window, INotifyPropertyChanged
    {
        public string LoginErrorText { get; set; }
        RemoteSpace AccountCreation;
        RemoteSpace loginSpace;
        public LogonWindow()
        {
            InitializeComponent();
            DataContext = this;
            AccountCreation = new RemoteSpace("tcp://10.16.169.224:5001/accountCreation?CONN");
            loginSpace = new RemoteSpace("tcp://10.16.169.224:5001/loginAttempts?CONN");
            UsernameInput.KeyUp += PasswordInput_KeyUp;
            PasswordInput.KeyUp += PasswordInput_KeyUp;
            SignupButton.Click += SignupButton_Click;
            UsernameInput.GotFocus += FocusClearError;
            PasswordInput.GotFocus += FocusClearError;
            UsernameInput.Focus();
            



        }

        private void FocusClearError(object sender, RoutedEventArgs e)
        {
            LoginErrorText = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            SignupWindow main = new SignupWindow( AccountCreation, loginSpace);
            App.Current.MainWindow = main;
            this.Close();
            main.Show();
        }

        private void PasswordInput_KeyUp(object sender, KeyEventArgs e)
        {
          if(e.Key == Key.Enter)
            {
               
              
                ParseInput(UsernameInput.Text.Trim(), PasswordInput.Password.Trim());
            }
        }

        //private void UsernameInput_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {

                
               
        //        ParseInput(UsernameInput.Text.Trim(), PasswordInput.Password.Trim());

        //    }
        //}

        private async void ParseInput(string Username, string Password)
        {
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Username))
            {
                // UsernameInput.Text = "Must enter a username";
                PasswordInput.Clear();
            }
            else if (!string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Username))
            {
                //login or create account depending on context
                var outcome = await Task<bool>.Factory.StartNew(() => authenticateLogin(Username, Password));

                if (outcome)
                {
                    ConferenceListWindow main = new ConferenceListWindow(Username);
                    App.Current.MainWindow = main;
                    this.Close();
                    main.Show();
                }
                else
                {
                    UsernameInput.Clear();
                    PasswordInput.Clear();
                    LoginErrorText = "Incorrect user/\npassword";
                    Keyboard.ClearFocus();
                }
            }
           
        }



      

        private bool authenticateLogin(string Username, string Password)
        {
            
            string PubKey = loginSpace.Query(typeof(string))[0] as string;
            //string EncryptedPassword = ;
            loginSpace.Put(Username, RSAEncrypt(Password, PubKey));
            var result = loginSpace.Get(Username, typeof(int)); 
            if (result != null)
            {
                if((int)result[1] == 1)
                {
                    return true;
                }
                else
                {
                    //handle wrong password
                    return false;
                }
            }
            else
            {
                //HANDLE LOGINSERVERERRORS
                return false;
            }
        }

        public static string RSAEncrypt(string Password, string destKey)
        {
            byte[] BytePass = Encoding.UTF8.GetBytes(Password);// Convert.FromBase64String(Password);
            byte[] encryptedData;
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(destKey);
            encryptedData = RSA.Encrypt(BytePass, true);
            RSA.Dispose();
            return Convert.ToBase64String(encryptedData);
        }





    }

}











//if (blnAuthenticateSuccessful) {
//    
//
//}