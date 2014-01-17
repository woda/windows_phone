using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Woda.Resources;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.IO.IsolatedStorage;

namespace Woda
{
    public partial class Login : PhoneApplicationPage
    {
        // Constructor
        public Login()
        {
            InitializeComponent();

            this.updateRequestBaseUrl();
            this.LoadLoginInformations();
        }

        private void updateRequestBaseUrl()
        {
            Request request = Request.Instance;
            request.SetBaseUrl(ServerAddress.Text);
            Debug.WriteLine("Base url updated to : " + ServerAddress.Text);
        }

        private  void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!this.Check_Fields())
                return;
            Data.Instance._LoginInformations = new Data.LoginInformations() { _Server = ServerAddress.Text, _Login = LoginBox.Text, _Password = Password.Password };
            Request.Instance._CookieSet = false;
            Data.Instance._NavigationFoldersIDs.Clear();
            Request request = Request.Instance;
            this.DoLogin(LoginBox.Text, Password.Password);
        }

        private void LoadLoginInformations()
        {
            IsolatedStorageSettings localStorage = IsolatedStorageSettings.ApplicationSettings;
            if (localStorage.Contains("LoginInformations"))
            {
                Data.LoginInformations infos = (Data.LoginInformations)localStorage["LoginInformations"];
                Request request = Request.Instance;
                request.SetBaseUrl(infos._Server);
                LoginBox.Text = infos._Login;
                ServerAddress.Text = infos._Server;
                Password.Password = infos._Password;
                this.DoLogin(infos._Login, infos._Password, false);
            }
        }

        private async void DoLogin(string login, string password, bool autologin = true)
        {
            LoginButton.IsEnabled = false;
            LoginButton.Content = "Connecting";
            Request request = Request.Instance;

            bool val = await request.LoginUser(login, password, autologin);

            LoginButton.Content = "Log in";
            LoginButton.IsEnabled = true; 
        }


        private bool Check_Fields()
        {
            if (String.IsNullOrWhiteSpace(LoginBox.Text))
            {
                MessageBox.Show("Empty field", LoginBox.Name, MessageBoxButton.OK);
                return false;
            }

            if (String.IsNullOrWhiteSpace(Password.Password))
            {
                MessageBox.Show("Empty field", Password.Name, MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        private void ServerAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.updateRequestBaseUrl();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            // cancel the navigation
            Application.Current.Terminate();
            e.Cancel = true;
        }
    }

}