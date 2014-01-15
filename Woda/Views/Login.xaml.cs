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

namespace Woda
{
    public partial class Login : PhoneApplicationPage
    {
        // Constructor
        public Login()
        {
            InitializeComponent();

            this.updateRequestBaseUrl();
        }

        private void updateRequestBaseUrl()
        {
            Request request = Request.Instance;
            request.SetBaseUrl(ServerAddress.Text);
            Debug.WriteLine("Base url updated to : " + ServerAddress.Text);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!this.Check_Fields())
                return;
            Request.Instance._CookieSet = false;
            Data.Instance._NavigationFoldersIDs.Clear();
            Request request = Request.Instance;
            request.LoginUser(LoginBox.Text, Password.Password);
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
            e.Cancel = true;
        }
    }
}