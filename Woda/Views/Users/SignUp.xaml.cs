using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;


namespace Woda.Views.Users
{
    public partial class SignUp : PhoneApplicationPage
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void Sign_up_Click(object sender, RoutedEventArgs e)
        {
            if (!this.Check_Fields())
                return;
            Request request = Request.Instance;
            request.CreateUser(Login.Text, Password.Text, Email.Text);
            Debug.WriteLine("Signup Clickeed");
        }

        private bool Check_Fields()
        {
            if (String.IsNullOrWhiteSpace(Login.Text))
            {
                MessageBox.Show("Empty field", Login.Name, MessageBoxButton.OK);
                return false;
            }

            if (String.IsNullOrWhiteSpace(Password.Text))
            {
                MessageBox.Show("Empty field", Password.Name, MessageBoxButton.OK);
                return false;
            }

            if (String.IsNullOrWhiteSpace(Email.Text))
            {
                MessageBox.Show("Empty field", Email.Name, MessageBoxButton.OK);
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}