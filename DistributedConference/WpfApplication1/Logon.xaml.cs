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
    public partial class Window1 : Window
    {
        string inputPassword;
        string inputUsername;
        public Window1()
        {
            InitializeComponent();
            UsernameInput.KeyUp += UsernameInput_KeyUp;
            PasswordInput.KeyUp += PasswordInput_KeyUp;
           
        }

        private void PasswordInput_KeyUp(object sender, KeyEventArgs e)
        {



          if(e.Key == Key.Enter)
            {
                inputPassword = PasswordInput.Password.Trim();
                inputUsername = UsernameInput.Text.Trim();
               // PasswordInput.Clear();
                ParseInput();
            }
        }

        private void UsernameInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                inputPassword = PasswordInput.Password.Trim();
                inputUsername = UsernameInput.Text.Trim();
                //UsernameInput.Clear();
                ParseInput();
            }
        }

        private void ParseInput()
        {
            if (string.IsNullOrWhiteSpace(inputUsername))
            {
               // UsernameInput.Text = "Must enter a username";
                PasswordInput.Clear();
            }
            if (string.IsNullOrWhiteSpace(inputPassword))
            {
                PasswordInput.Clear();
                //errors
            }
            else if (!string.IsNullOrWhiteSpace(inputPassword) && !string.IsNullOrWhiteSpace(inputUsername))
            {
                //login or create account depending on context
                if (authenticateLogin())
                {
                    MainWindow main = new MainWindow();
                    App.Current.MainWindow = main;
                    this.Close();
                    main.Show();
                }
            }
           
        }

        private bool authenticateLogin()
        {
            //TEMPORARY MUST BE IMPLEMENTED
            return true;
        }
    }
}











//if (blnAuthenticateSuccessful) {
//    
//
//}