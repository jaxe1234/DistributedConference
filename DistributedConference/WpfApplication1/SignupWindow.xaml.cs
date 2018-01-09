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
    /// Interaction logic for SignupWindow.xaml
    /// </summary>
    public partial class SignupWindow : Window
    {
        RemoteSpace AccountCreation;
        RemoteSpace loginSpace;


        public SignupWindow(RemoteSpace AccountCreation, RemoteSpace loginSpace)
        {
            this.AccountCreation = AccountCreation;
            this.loginSpace = loginSpace;
            InitializeComponent();
            UsernameInput.KeyUp += PasswordInput_KeyUp;
            PasswordInput.KeyUp += PasswordInput_KeyUp;
            PasswordRepeat.KeyUp += PasswordInput_KeyUp;
        }



        private void PasswordInput_KeyUp(object sender, KeyEventArgs e)
        {



            if (e.Key == Key.Enter)
            {


                ParseInput(UsernameInput.Text.Trim(), PasswordInput.Password.Trim(), PasswordRepeat.Password.Trim());
            }
        }


        private void ParseInput(string Username, string Password, string Repeat)
        {
            if (Password == Repeat)
            {
                if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Username))
                {
                    // UsernameInput.Text = "Must enter a username";
                    PasswordInput.Clear();
                    PasswordRepeat.Clear();
                }
                else if (!string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Username))
            {
                    //login or create account depending on context
                    if (AuthenticateAccountCreate(Username, Password))
                    {
                        LogonWindow main = new LogonWindow();
                        App.Current.MainWindow = main;
                        UsernameInput.Clear();
                        PasswordInput.Clear();
                        PasswordRepeat.Clear();
                        this.Close();
                        main.Show();
                    }
                    else
                    {
                        UsernameInput.Clear();
                        PasswordInput.Clear();
                        PasswordRepeat.Clear();
                    }
                }
            }
            else
            {
                UsernameInput.Text = "User alredy exists";
            }


        }




        private bool AuthenticateAccountCreate(string Username, string Password)
        {
            //TEMPORARY MUST BE IMPLEMENTED
            AccountCreation.Put(Username, Password);
            var result = AccountCreation.GetP(Username, typeof(int));
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
