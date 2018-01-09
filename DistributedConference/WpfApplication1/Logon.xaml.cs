using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class LogonWindow : Window
    {
        
        RemoteSpace AccountCreation;
        RemoteSpace loginSpace;
        public LogonWindow()
        {
            InitializeComponent();
            AccountCreation = new RemoteSpace("tcp://10.16.169.224:5001/accountCreation");
            loginSpace = new RemoteSpace("tcp://10.16.169.224:5001/loginAttempts");
            UsernameInput.KeyUp += PasswordInput_KeyUp;
            PasswordInput.KeyUp += PasswordInput_KeyUp;
            SignupButton.Click += SignupButton_Click;
           
        }

        private void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            SignupWindow main = new SignupWindow(AccountCreation, loginSpace);
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

        private void ParseInput(string Username, string Password)
        {
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Username))
            {
                // UsernameInput.Text = "Must enter a username";
                PasswordInput.Clear();
            }
            else if (!string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Username))
            {
                //login or create account depending on context
                if (authenticateLogin(Username, Password))
                {
                    MainWindow main = new MainWindow();
                    App.Current.MainWindow = main;
                    this.Close();
                    main.Show();
                }
                else
                {
                    UsernameInput.Clear();
                    PasswordInput.Clear();
                }
            }
           
        }

        private bool authenticateLogin(string Username, string Password)
        {
            //TEMPORARY MUST BE IMPLEMENTED
            loginSpace.Put(Username, Password);
            var result = loginSpace.GetP(Username, typeof(int));
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
    }
}











//if (blnAuthenticateSuccessful) {
//    
//
//}