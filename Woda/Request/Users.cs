using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using RestSharp;
using System.Net;
using System.Windows.Navigation;

namespace Woda
{
    public sealed partial class Request
    {
        // method to create and log a new user in.
        public void CreateUser(string login, string password, string email)
        {
            var request = new RestRequest("users/{login}", Method.PUT);

            request.AddUrlSegment("login", login);

            request.AddParameter("email", email);
            request.AddParameter("password", password);

            var asyncHandle = _Client.ExecuteAsync<UserResponse>(request, response =>
            {
                if (!this.HandleResponse<UserResponse>(response))
                    return;
              Data.Instance._User = response.Data.user;

              Debug.WriteLine("Signed up and Logged in as : " + Data.Instance._User.login);
              App.RootFrame.Navigate(new Uri("/Views/Users/Home.xaml", UriKind.Relative));
            });

        }

        // method to get the information of the current user or of a specific user if its id is specified
        public void ReadUser(string id = "")
        {
            var request = new WRequest("users", Method.GET);

            if (!String.IsNullOrEmpty(id))
                request.AddParameter("id", id);

            var asyncHandle = _Client.ExecuteAsync<UserResponse>(request, response =>
            {
                if (!this.HandleResponse<UserResponse>(response))
                    return;
            });

        }

        // method to log a user in
        public void LoginUser(string login, string password)
        {
            var request = new RestRequest("users/{login}/login", Method.POST);
        
            request.AddUrlSegment("login", login);

            request.AddParameter("password", password);

            var asyncHandle = _Client.ExecuteAsync<UserResponse>(request, response =>
           {
                if (!this.HandleResponse<UserResponse>(response))
                    return;

                Data.Instance._User = response.Data.user;

                Debug.WriteLine("Logged in as : " + Data.Instance._User.login);
                App.RootFrame.Navigate(new Uri("/Views/Users/Home.xaml", UriKind.Relative));
            });
        }

        // method to log a user out
        public void LogoutUser()
        {
            var request = new WRequest("users/logout", Method.GET);
            var asyncHandle = _Client.ExecuteAsync<UserResponse>(request, response =>
            {
                if (!this.HandleResponse<UserResponse>(response))
                    return;
                App.RootFrame.Navigate(new Uri("/Views/Login.xaml", UriKind.Relative));
               _CookieSet = false;
                Data.Instance._NavigationFoldersIDs.Clear();
            });
        }
    }
}
