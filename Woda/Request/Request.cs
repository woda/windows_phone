using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using RestSharp;

using System.Windows;
using System.Net;

namespace Woda
{
    public sealed partial class Request
    {

       private static volatile Request instance;
       private static object syncRoot = new Object();
       private RestClient _Client;
       public String _BaseUrl { get; set; }
       public bool _CookieSet { get; set; }

       private Request()
       {
           _Client = new RestClient();
           _CookieSet = false;
       }

       public static Request Instance
       {
           get
           {
               if (instance == null)
               {
                   lock (syncRoot)
                   {
                       if (instance == null)
                           instance = new Request();
                   }
               }
               return instance;
           }
       }

       private bool HandleResponse<T>(RestSharp.IRestResponse<T> response) where T : Response
       {
           Debug.WriteLine(response.Content);

           if (response.Data == null)
           {
               MessageBox.Show("Server is not responding", "Error", MessageBoxButton.OK);
               return false;    
           }

           if (!String.IsNullOrEmpty(response.Data.error))
           {
               MessageBox.Show(response.Data.message, "Error" , MessageBoxButton.OK);
               return false;
           }

           if (response.StatusCode != System.Net.HttpStatusCode.OK)
               return false;

           this.SetCookie(response);

           if (!response.Data.success)
               return false;
           return true;
       }

       private void SetCookie<T>(RestSharp.IRestResponse<T> response) where T : Response
        {
            if (_CookieSet)
                return;
         
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                foreach (var i in response.Headers)
                {
                    if (i.Name == "Set-Cookie")
                    {
                        var cookie = i.Value.ToString().Split(';');
                        Data.Instance._SessionCookie = new Cookie(cookie[0].Split('=')[0], cookie[0].Split('=')[1]);
                        break;
                    }
                }
            }
            _CookieSet = true;
            Debug.WriteLine("Cookie set");
        }

        public void SetBaseUrl(string url)
        {
            _BaseUrl = url;
            _Client.BaseUrl = _BaseUrl;
        }
    }

    public class Cookie
    {
        public Cookie(string name, string value) { _Name = name; _Value = value; }
        public string _Name {get; set;}
        public string _Value {get; set;}
    }

    public class WRequest : RestRequest
    {
        public WRequest(string resource, Method method)
            : base(resource, method)
            {
                this.AddParameter(Data.Instance._SessionCookie._Name, Data.Instance._SessionCookie._Value, ParameterType.Cookie);
                this.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            }
    }
}
