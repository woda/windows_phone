using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Input;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using System.Windows.Controls.Primitives;
using Windows;
using System.Security.Cryptography;
using System.IO;

namespace Woda.Views.Users
{
    public partial class Home : PhoneApplicationPage
    {
        public object _CurrentMenu { get; set; }

        public Home()
        {
            InitializeComponent();
            this.InitializeApplicationBar();
            this.InitializeContextMenu();
            this.Home_Click(null, null);
        }

        private void GoToFolder(int id = -1)
        {
            this.GetFolder(id);
        }

        private void GoToParentFolder(bool reloadFolder = false)
        {
            if (Data.Instance._NavigationFoldersIDs.Count() <= 1)
            {
                switch (PageTitle.Text)
                {
                    case "Favorites":
                        Debug.WriteLine("Reload favorites");
                        this.GetFavorites();
                        break;
                    case "Public Files":
                        Debug.WriteLine("Reload public");
                        this.GetPublic();
                        break;
                    case "Recent Files":
                        Debug.WriteLine("Reload recent");
                        this.GetRecent();
                        break;
                    default:
                        if (Data.Instance._NavigationFoldersIDs.Count() == 1)
                        {
                            Debug.WriteLine("Reload current folder");
                            int id = Data.Instance._NavigationFoldersIDs.ElementAt(0);
                            this.GetFolder(id, false);
                        }
                        break;
                }
            }
            else
            {
                if (!reloadFolder)
                    Data.Instance._NavigationFoldersIDs.RemoveAt(0);
                int id = Data.Instance._NavigationFoldersIDs.ElementAt(0);
                this.GetFolder(id, false);
            }
        }

        private async void ShowImage(int id)
        {
            Request request = Request.Instance;

            string link = await request.DLLFiles(id);

            if (link == null)
                return;

            Debug.WriteLine(link);
            App.RootFrame.Navigate(new Uri("/Views/Files/Image.xaml?link=" + link, UriKind.Relative));
        }

        private async void ShowMedia(int id, string MediaType)
        {
            Request request = Request.Instance;

            string link = await request.DLLFiles(id);

            if (link == null)
                return;
            MediaPlayerLauncher mediaPlayerLauncher = new MediaPlayerLauncher();

            mediaPlayerLauncher.Media = new Uri(link, UriKind.Absolute);
            mediaPlayerLauncher.Location = MediaLocationType.Data;

            if (MediaType == @"video")
            {
                mediaPlayerLauncher.Orientation = MediaPlayerOrientation.Landscape;
                mediaPlayerLauncher.Controls = MediaPlaybackControls.Pause | MediaPlaybackControls.Stop;
            }
            else
            {
                mediaPlayerLauncher.Orientation = MediaPlayerOrientation.Portrait;
                mediaPlayerLauncher.Controls = MediaPlaybackControls.None;
            }

            mediaPlayerLauncher.Show();
        }

        // Copy the download link to the clipboard
        private async void CopyLinkToClipboard(int id)
        {
            Request request = Request.Instance;

            string link = await request.DLLFiles(id);

            if (link == null)
                return;
            Clipboard.SetText(link);
            MessageBox.Show("The download link has be copied to the clipboard", "Success", MessageBoxButton.OK);
        }

        private void FilesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (FilesList.SelectedIndex == -1)
                return;

            Debug.WriteLine("File selected : " + FilesList.SelectedIndex.ToString());

            // Going to selected folder

            // If it's a folder
            if (((Data.File)(FilesList.SelectedItem))._File.GetType() == typeof(Folder))
            {
                Debug.WriteLine("Folder");
                Folder folder = (Folder)(((Data.File)(FilesList.SelectedItem))._File);
                Debug.WriteLine("ID : " + folder.id);
                this.GoToFolder(folder.id);
            }
            // If the file is a folder
            else if (((File)(((Data.File)(FilesList.SelectedItem))._File)).folder)
            {
                Debug.WriteLine("Folder");
                File folder = (File)(((Data.File)(FilesList.SelectedItem))._File);
                Debug.WriteLine("ID : " + folder.id);
                this.GoToFolder(folder.id);
            }
            // Displaying file depending on format
            else
            {
                Debug.WriteLine("File");
                File file = (File)(((Data.File)(FilesList.SelectedItem))._File);
                Debug.WriteLine("ID : " + file.id);

                string format = null;

                // Getting file format
                if (Data._Types.ContainsKey(file.type.ToUpper()))
                {
                    string icon = Data._Types[file.type.ToUpper()];
                    format = Data._FileFormat.FirstOrDefault(x => x.Value == icon).Key;
                }

                if (format == @"image")
                    this.ShowImage(file.id);
                else if (format == @"video" || format == @"music")
                    this.ShowMedia(file.id, format);
                else
                    this.CopyLinkToClipboard(file.id);
            }
            // Reset selected index to -1 (no selection)
            FilesList.SelectedIndex = -1;
        }

        private async Task<Uri> GetLink(int id)
        {
            var tcs = new TaskCompletionSource<Uri>();

            Request request = Request.Instance;

            string link = await request.DLLFiles(id);

            if (link == null)
                tcs.SetResult(null);
            else
                tcs.SetResult(new Uri(link, UriKind.Absolute));
            return await tcs.Task;
        }

        public async void GetFavorites()
        {
            Request request = Request.Instance;
            FilesResponse response = await request.FavoritesFiles();

            if (response == null)
                return;
            this.UpdateList(response.files);
        }

        public async void GetRecent()
        {
            Request request = Request.Instance;
            FilesResponse response = await request.RecentsFiles();

            if (response == null)
                return;
            this.UpdateList(response.files);
        }

        public async void GetPublic()
        {
            Request request = Request.Instance;
            FilesResponse response = await request.PublicListFiles();

            if (response == null)
                return;
            this.UpdateList(response.files);
        }

        public async void SetFavorite(Data.File item)
        {
            Request request = Request.Instance;
            GenericFile file = (GenericFile)item._File;

            FileResponse response = await request.SetFavoriteFiles(file.id, !file.favorite);

            if (response == null)
                return;

            file = response.file;
            if (response.success)
                this.GoToParentFolder(true);
        }

        public async void SetPublic(Data.File item)
        {
            Request request = Request.Instance;
            GenericFile file = (GenericFile)item._File;

            FileResponse response = await request.SetPublicFiles(file.id, !file.@public);

            if (response == null)
                return;

            file = response.file;
            if (response.success)
                this.GoToParentFolder(true);
        }

        public async void DeleteFile(Data.File item)
        {
            Request request = Request.Instance;
            GenericFile file = (GenericFile)item._File;

            bool response = await request.DeleteFile(file.id);

            if (response)
                this.GoToParentFolder(true);
        }

        private void AddFolder(ref List<Data.File> list, GenericFile folder)
        {
            if (folder.folder)
            {
                string date = System.Convert.ToDateTime(folder.last_update).ToString();
                string favoriteIcon = folder.favorite ? "/Resources/Images/star_yellow.png" : "/Resources/Images/star_white.png";
                string publicIcon = folder.@public ? "/Resources/Images/eye_open.png" : "/Resources/Images/eye_closed.png";
                list.Add(new Data.File() { _Name = folder.name, _SizeAndDate = "Last update : " + date, _Icon = Data._FileFormat["folder"], _File = folder, _PublicIcon = publicIcon, _FavoriteIcon = favoriteIcon });
            }
        }

        private void AddFile(ref List<Data.File> list, File file)
        {
            if (!file.folder)
            {
                string date = System.Convert.ToDateTime(file.last_update).ToString();
                string favoriteIcon = file.favorite ? "/Resources/Images/star_yellow.png" : "/Resources/Images/star_white.png";
                string publicIcon = file.@public ? "/Resources/Images/eye_open.png" : "/Resources/Images/eye_closed.png";
                string icon = Data._Types.ContainsKey(file.type.ToUpper()) ? Data._Types[file.type.ToUpper()] : Data._FileFormat["default"];
                list.Add(new Data.File() { _Name = file.name, _SizeAndDate = (file.size / 1000).ToString() + "KB" + " last update : " + date, _Icon = icon, _File = file, _PublicIcon = publicIcon, _FavoriteIcon = favoriteIcon });
            }
        }

        private void UpdateList(List<File> Files)
        {

            List<Data.File> files = new List<Data.File>();

            if (Files.Count() == 0)
            {

            }

            // Adding the folders first
            foreach (GenericFile folder in Files)
                this.AddFolder(ref files, folder);

            // Adding files
            foreach (File file in Files)
                this.AddFile(ref files, file);

            FilesList.ItemsSource = files;
        }

        public async void GetFolder(int id = -1, bool AddToNavigation = true)
        {
            Request request = Request.Instance;
            bool val;
            try
            {
                val = await request.ListFiles(id);

            }
            catch
            {
                val = false;
            }


            if (!val)
                return;

            if (AddToNavigation)
                Data.Instance._NavigationFoldersIDs.Insert(0, Data.Instance._CurrentFolder.id);

            List<Data.File> files = new List<Data.File>();

            // Adding folders
            foreach (GenericFile folder in Data.Instance._CurrentFolder.folders)
                this.AddFolder(ref files, folder);

            // Adding files
            foreach (File file in Data.Instance._CurrentFolder.files)
                this.AddFile(ref files, file);

            FilesList.ItemsSource = files;
        }

        // Folder and File upload

        private void ButtonAddFolder_Click(object sender, RoutedEventArgs e)
        {
            TextBox textBox = new TextBox();

            CustomMessageBox box = new CustomMessageBox()
            {
                Caption = "Add folder",
                Message = "Enter a unique name for the new folder.",
                LeftButtonContent = "ok",
                RightButtonContent = "cancel",
                Content = textBox
            };
            box.Dismissed += (s, boxEventArgs) =>
            {
                if (!String.IsNullOrWhiteSpace(textBox.Text))
                    this.CreateFolder(textBox.Text);
            };
            box.Show();
        }

        private string GetPath(List<Folder> folders)
        {
            string path = "";
            foreach (Folder folder in folders)
            {
                path += folder.name;
                if (folder.name != "/")
                    path += "/";
            }
            return path;
        }

        private async void CreateFolder(string name)
        {
            BreadcrumbResponse response = await Request.Instance.BreadcrumbFiles(Data.Instance._CurrentFolder.id);

            if (response == null)
                return;

            string path = this.GetPath(response.breadcrumb);
            path += name;
            FolderResponse newFolder = await Request.Instance.CreateFolder(path);

            if (newFolder != null)
                this.GoToFolder(newFolder.folder.id);
        }

        private void ButtonAddFile_Click(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask selectphoto = new PhotoChooserTask();
            selectphoto.Completed += new EventHandler<PhotoResult>(UploadSelectedFile);
            selectphoto.Show();
        }

        private async void UploadSelectedFile(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                SHA256 sha256 = new SHA256Managed();

                System.IO.Stream fileStream = e.ChosenPhoto;

                // Getting hash SHA256
                byte[] hashValue;
                fileStream.Position = 0;
                hashValue = sha256.ComputeHash(fileStream);

                string hex = BitConverter.ToString(hashValue);
                string content_hash = hex.Replace("-", "");

                string fileName = System.IO.Path.GetFileName(e.OriginalFileName);
                fileName = System.IO.Path.ChangeExtension(fileName, "bmp");

                // Getting current folder path
                BreadcrumbResponse breadcrumb = await Request.Instance.BreadcrumbFiles(Data.Instance._CurrentFolder.id);
                if (breadcrumb == null)
                    return;
                string path = this.GetPath(breadcrumb.breadcrumb);
                path += fileName;

                // Getting filesize
                long size = e.ChosenPhoto.Length;

                Debug.WriteLine(path);
                Debug.WriteLine(size.ToString());
                Debug.WriteLine(content_hash);

                CreateFileResponse response = await Request.Instance.CreateFile(path, content_hash, size);

                if (response == null)
                    return;
                if (response.uploaded)
                {
                    MessageBox.Show("File already uploaded", fileName, MessageBoxButton.OK);
                    return;
                }

                // Read file 

                fileStream.Position = 0;
                long numBytesToRead = fileStream.Length;
                int numBytesRead = 0;
                byte[] chunk = new byte[response.part_size];
                int part_number = 0;
                while (numBytesToRead > 0)
                {
                    int index = 0;
                    foreach (byte b in chunk)
                    {
                        chunk[index] = 0;
                        index++;
                    }
                    // get a chunk
                    Debug.WriteLine("Getting chunk : " + part_number);

                    int n = fileStream.Read(chunk, numBytesRead, response.part_size);
                    if (n == 0)
                    {
                        break;
                    }
                    // if the part number is needed, we send it
                    if (response.needed_parts.Contains(part_number))
                    {
                        Debug.WriteLine("Part needed : " + part_number);
                        UploadFileResponse uploadFileResponse = await Request.Instance.UploadFile(response.file.id, part_number, chunk, n);
                        if (uploadFileResponse == null)
                        {
                            MessageBox.Show("Upload failed, please try again", fileName, MessageBoxButton.OK);
                            fileStream.Close();
                            return;
                        }
                        else if (uploadFileResponse.uploaded)
                        {
                            Debug.WriteLine("File uploaded ! ");
                            break;
                        }
                    }
                    part_number += 1;
                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                fileStream.Close();
                this.GoToParentFolder(true);
            }
        }

        // Navigation Methods

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            this.GoToParentFolder();
            // cancel the navigation
            e.Cancel = true;
        }

        private void Home_Click(object sender, EventArgs e)
        {
            PageTitle.Text = "Files";
            Data.Instance._NavigationFoldersIDs.Clear();
            this.GoToFolder();
        }

        private void MenuFavorites_Click(object sender, EventArgs e)
        {
            PageTitle.Text = "Favorites";
            Data.Instance._NavigationFoldersIDs.Clear();
            this.GetFavorites();
        }

        private void MenuRecent_Click(object sender, EventArgs e)
        {
            PageTitle.Text = "Recent Files";
            Data.Instance._NavigationFoldersIDs.Clear();
            this.GetRecent();
        }

        private void MenuPublic_Click(object sender, EventArgs e)
        {
            PageTitle.Text = "Public Files";
            Data.Instance._NavigationFoldersIDs.Clear();
            this.GetPublic();
        }

        private void MenuLogout_Click(object sender, EventArgs e)
        {
            Request request = Request.Instance;

            request.LogoutUser();
        }

        private void MenuFAQ_Click(object sender, EventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://woda.ws/FAQ.html");
            webBrowserTask.Show();
        }

        private void MenuLegalNotice_Click(object sender, EventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://woda.ws/legalnotice.html");
            webBrowserTask.Show();
        }

        private void MenuContact_Click(object sender, EventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://woda.ws/contact.html");
            webBrowserTask.Show();
        }

        // Call the context menu from the held item
        private void FilesList_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)e.OriginalSource;
            Data.File item = (Data.File)element.DataContext;

            if (item == null)
                return;

            Debug.WriteLine("File holded : " + item._Name);
            Debug.WriteLine("File holded : " + FilesList.Items.IndexOf(item));

            ContextMenuService.SetContextMenu(element, _ContextMenu);

            foreach (MenuItem menuItem in _ContextMenu.Items)
                menuItem.Tag = item;

            if (((GenericFile)item._File).favorite)
                ((MenuItem)_ContextMenu.Items.ElementAt(0)).Header = "Remove from favorites";
            else
                ((MenuItem)_ContextMenu.Items.ElementAt(0)).Header = "Add to favorites";

            if (((GenericFile)item._File).@public)
                ((MenuItem)_ContextMenu.Items.ElementAt(1)).Header = "Remove from public files";
            else
                ((MenuItem)_ContextMenu.Items.ElementAt(1)).Header = "Add to public files";

            _ContextMenu.IsOpen = true;

        }

        // Context Menu methods

        private void SetFavorite_Click(object sender, RoutedEventArgs e)
        {
            Data.File item = (Data.File)((MenuItem)sender).Tag;
            Debug.WriteLine(item._Name);
            this.SetFavorite(item);
        }

        private void SetPublic_Click(object sender, RoutedEventArgs e)
        {
            Data.File item = (Data.File)((MenuItem)sender).Tag;
            Debug.WriteLine(item._Name);
            this.SetPublic(item);
        }

        private async void ShareSocial_Click(object sender, RoutedEventArgs e)
        {
            Data.File item = (Data.File)((MenuItem)sender).Tag;
            Debug.WriteLine(item._Name);

            GenericFile file = (GenericFile)item._File;

            Uri link = await this.GetLink(file.id);

            if (link == null)
                return;

            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.LinkUri = link;
            shareLinkTask.Message = "Download this file!";
            shareLinkTask.Show();
        }

        private async void ShareEmail_Click(object sender, RoutedEventArgs e)
        {
            Data.File item = (Data.File)((MenuItem)sender).Tag;
            Debug.WriteLine(item._Name);

            GenericFile file = (GenericFile)item._File;

            Uri link = await this.GetLink(file.id);

            if (link == null)
                return;

            EmailComposeTask task = new EmailComposeTask();
            task.Subject = "Download this file!";
            task.Body = "Hey, download this awesome file ! " + link.ToString();
            task.Show();
        }

        private void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            Data.File item = (Data.File)((MenuItem)sender).Tag;
            Debug.WriteLine(item._Name);
            this.DeleteFile(item);
        }

        private void InitializeContextMenu()
        {
            _ContextMenu = new ContextMenu();
            _ContextMenu.Padding = new Thickness(0);
            _ContextMenu.Background = new SolidColorBrush() { Color = Color.FromArgb(255, 70, 135, 255) };

            MenuItem favorites = new MenuItem() { Header = "Favorites", Foreground = new SolidColorBrush() { Color = Color.FromArgb(255, 255, 255, 255) } };
            favorites.Click += SetFavorite_Click;
            _ContextMenu.Items.Add(favorites);

            MenuItem @public = new MenuItem() { Header = "Public", Foreground = new SolidColorBrush() { Color = Color.FromArgb(255, 255, 255, 255) } };
            @public.Click += SetPublic_Click;
            _ContextMenu.Items.Add(@public);

            MenuItem shareSocial = new MenuItem() { Header = "Share", Foreground = new SolidColorBrush() { Color = Color.FromArgb(255, 255, 255, 255) } };
            shareSocial.Click += ShareSocial_Click;
            _ContextMenu.Items.Add(shareSocial);

            MenuItem shareEmail = new MenuItem() { Header = "Email", Foreground = new SolidColorBrush() { Color = Color.FromArgb(255, 255, 255, 255) } };
            shareEmail.Click += ShareEmail_Click;
            _ContextMenu.Items.Add(shareEmail);

            MenuItem delete = new MenuItem() { Header = "Delete", Foreground = new SolidColorBrush() { Color = Color.FromArgb(255, 255, 255, 255) } };
            delete.Click += DeleteFile_Click;
            _ContextMenu.Items.Add(delete);
        }

        private void InitializeApplicationBar()
        {
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = "Login : " + Data.Instance._User.login;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).Text = "Server : " + Request.Instance._BaseUrl;

            var usedSpace = Math.Round((Data.Instance._User.used_space / 1024f / 1024f), 2);
            var space = Math.Round((Data.Instance._User.space / 1024f / 1024f), 2);
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[2]).Text = "Space : " + usedSpace.ToString() + " MB / " + space.ToString() + "MB";
        }

        public ContextMenu _ContextMenu { get; set; }
        private Popup _Popup { get; set; }
    }
}