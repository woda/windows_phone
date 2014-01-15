
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Phone.Controls;

namespace Woda.Views.Files
{
    public partial class Image : PhoneApplicationPage
    {
    public Image()
    {
        InitializeComponent();
    }

    private void LoadImage(string url)
    {

        var bitmap = new BitmapImage(new Uri(url));
        bitmap.ImageFailed += (s, e) => this.ShowError("Error while loading the image.");
        bitmap.ImageOpened += (s, e) => this.ShowImage();

        this.ShowLoading();
        this.imageView.Source = bitmap;
    }

    protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        string link;

        if (NavigationContext.QueryString.TryGetValue("link", out link))
        {
            Debug.WriteLine(link);
            this.LoadImage(link);
        }
    }


    private void ShowLoading()
    {
        this.loadingView.Visibility = Visibility.Visible;
        this.imageView.Visibility = Visibility.Collapsed;
        this.errorView.Visibility = Visibility.Collapsed;
    }
 
    private void ShowError(string message)
    {
        this.loadingView.Visibility = Visibility.Collapsed;
        this.imageView.Visibility = Visibility.Collapsed;
        this.errorView.Visibility = Visibility.Visible;
 
        this.errorView.Text = message;
    }
 
    private void ShowImage()
    {
        this.loadingView.Visibility = Visibility.Collapsed;
        this.imageView.Visibility = Visibility.Visible;
        this.errorView.Visibility = Visibility.Collapsed;
    }
    }
}