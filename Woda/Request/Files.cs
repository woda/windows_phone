using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using RestSharp;

namespace Woda
{
    public sealed partial class Request
    {
        // method to get all files and folders of the current user.
        //public void ListFiles(int id = -1, int depth = 0)
        public async Task<bool> ListFiles(int id = -1, int depth = 0)
        {
            var tcs = new TaskCompletionSource<bool>();
            var request = new WRequest("files/{id}", Method.GET);

            if (id >= 0)
                request.AddUrlSegment("id", id.ToString());
            else
                request.AddUrlSegment("id", "");

            request.AddParameter("depth", depth.ToString());

            Debug.WriteLine(request.ToString());
            var asyncHandle = _Client.ExecuteAsync<FolderResponse>(request, response =>
            {
                if (!this.HandleResponse<FolderResponse>(response))
                    tcs.SetResult(false);
                else
                {
                    Data.Instance._CurrentFolder = response.Data.folder;
                    Debug.WriteLine("Getting all files and folder of the current user : " + Data.Instance._User.login);
                    tcs.SetResult(true);
                }
            });
            return await tcs.Task;
        }

        // method to get the public hierarchy of another user.

        public void UsersFiles(int userID, int id = 0, int depth = 0)
        {
            var request = new WRequest("usersfile/{user id}/{id}", Method.GET);

            request.AddUrlSegment("user id", userID.ToString());

            if (id >= 0)
                request.AddUrlSegment("id", id.ToString());
            else
                request.AddUrlSegment("id", "");

            request.AddParameter("depth", depth.ToString());

            var asyncHandle = _Client.ExecuteAsync<FolderResponse>(request, response =>
            {
                if (!this.HandleResponse<FolderResponse>(response))
                    return;
                Debug.WriteLine("Getting all public files and folder of the current user : " + Data.Instance._User.login);
            });
        }

        // method to get the user's recent files and 

        public async Task<FilesResponse> RecentsFiles()
        {
            var tcs = new TaskCompletionSource<FilesResponse>();

            var request = new WRequest("files/recents", Method.GET);

            var asyncHandle = _Client.ExecuteAsync<FilesResponse>(request, response =>
            {
                if (!this.HandleResponse<FilesResponse>(response))
                {
                    tcs.SetResult(null);
                    return;
                }
                Debug.WriteLine("Getting all recents files and folder of the current user : " + Data.Instance._User.login);
                tcs.SetResult(response.Data);
            });
            return await tcs.Task;
        }

        // method to get the user's favorite files and folders
        public async Task<FilesResponse> FavoritesFiles()
        {
            var tcs = new TaskCompletionSource<FilesResponse>();

            var request = new WRequest("files/favorites", Method.GET);

            var asyncHandle = _Client.ExecuteAsync<FilesResponse>(request, response =>
            {
                if (!this.HandleResponse<FilesResponse>(response))
                {
                    tcs.SetResult(null);
                    return;
                }
                tcs.SetResult(response.Data);

                Debug.WriteLine("Getting all favorites files and folder of the current user : " + Data.Instance._User.login);
            });
            return await tcs.Task;
        }

        // method to set a user's file or folder as favorite
        public async Task<FileResponse> SetFavoriteFiles(int fileID, bool value)
        {
            var tcs = new TaskCompletionSource<FileResponse>();
            var request = new WRequest("files/favorites/{file id}", Method.POST);

            request.AddUrlSegment("file id", fileID.ToString());

            request.AddParameter("favorite", value.ToString().ToLower());

            var asyncHandle = _Client.ExecuteAsync<FileResponse>(request, response =>
            {
                if (!this.HandleResponse<FileResponse>(response))
                {
                    tcs.SetResult(null);
                    return;
                }
                Debug.WriteLine("Setting favorite file or folder of the current user : " + Data.Instance._User.login);
                tcs.SetResult(response.Data);
            });
            return await tcs.Task;
        }

        // method to get the user's public files and folders
        public Task<FilesResponse> PublicListFiles()
        {
            var tcs = new TaskCompletionSource<FilesResponse>();

            var request = new WRequest("files/public", Method.GET);

            var asyncHandle = _Client.ExecuteAsync<FilesResponse>(request, response =>
            {
                if (!this.HandleResponse<FilesResponse>(response))
                {
                    tcs.SetResult(null);
                    return;
                }
                tcs.SetResult(response.Data);
                Debug.WriteLine("Getting all public files and folder of the current user : " + Data.Instance._User.login);
            });
            return tcs.Task;
        }

        // method to set a user's file of folder as public
        public async Task<FileResponse> SetPublicFiles(int fileID, bool value)
        {
            var tcs = new TaskCompletionSource<FileResponse>();

            var request = new WRequest("files/public/{file id}", Method.POST);

            request.AddUrlSegment("file id", fileID.ToString());

            request.AddParameter("public", value.ToString().ToLower());

            var asyncHandle = _Client.ExecuteAsync<FileResponse>(request, response =>
            {
                if (!this.HandleResponse<FileResponse>(response))
                {
                    tcs.SetResult(null);
                    return;
                }
                Debug.WriteLine("Setting public file or folder of the current user : " + Data.Instance._User.login);
                tcs.SetResult(response.Data);
            });
            return await tcs.Task;
        }

        // method to share a specific file to a selected user. The file will appears into the new user's hierarchy as a linked file and not as a copy one.
        public void ShareFiles(int fileID, string userLogin)
        {
            var request = new WRequest("files/share/{file id}", Method.POST);

            request.AddUrlSegment("file id", fileID.ToString());

            request.AddParameter("login", userLogin.ToString());

            var asyncHandle = _Client.ExecuteAsync<FolderResponse>(request, response =>
            {
                if (!this.HandleResponse<FolderResponse>(response))
                    return;
                Debug.WriteLine("Sharing file or folder of the current user : " + Data.Instance._User.login);
            });
        }


        // method to get the user's shared files and folders. A shared file is a file which had many different users accessing it

        public async Task<FilesResponse> SharedFiles(int id = -1)
        {
            var tcs = new TaskCompletionSource<FilesResponse>();
            var request = new WRequest("files/shared/{id}", Method.GET);

            if (id >= 0)
                request.AddUrlSegment("id", id.ToString());
            else
                request.AddUrlSegment("id", "");

            Debug.WriteLine(request.ToString());
            var asyncHandle = _Client.ExecuteAsync<FilesResponse>(request, response =>
            {
                if (!this.HandleResponse<FilesResponse>(response))
                {
                    tcs.SetResult(null);
                }
                tcs.SetResult(response.Data);
                Debug.WriteLine("Getting all shared files and folder of the current user : " + Data.Instance._User.login);
            });
            return await tcs.Task;
        }

        // method to get the direct download link of a file or a folder.
        public async Task<string> DLLFiles(int fileID)
        {
            var tcs = new TaskCompletionSource<string>();
            var request = new WRequest("files/link/{file id}", Method.GET);

            request.AddUrlSegment("file id", fileID.ToString());

            Debug.WriteLine(request.ToString());
            var asyncHandle = _Client.ExecuteAsync<DirectDownloadLink>(request, response =>
            {
                if (!this.HandleResponse<DirectDownloadLink>(response))
                {
                    tcs.SetResult(null);
                    return;
                }
                tcs.SetResult(response.Data.link);
                Debug.WriteLine("Getting DDL of file or folder");
            });
            return await tcs.Task;
        }

        // method to get the list of direct download links generated by the user for its files.
        public void MyLinksFiles()
        {
            var request = new WRequest("files/mylinks", Method.GET);

            Debug.WriteLine(request.ToString());
            var asyncHandle = _Client.ExecuteAsync<FolderResponse>(request, response =>
            {
                if (!this.HandleResponse<FolderResponse>(response))
                    return;
                Debug.WriteLine("Getting DDL of file or folder");
            });
        }

        // method to get the list of direct download links generated by the user for its files.
        public async Task<FilesResponse> DownloadedFiles()
        {
            var tcs = new TaskCompletionSource<FilesResponse>();

            var request = new WRequest("files/downloaded", Method.GET);

            Debug.WriteLine(request.ToString());
            var asyncHandle = _Client.ExecuteAsync<FilesResponse>(request, response =>
            {
                if (!this.HandleResponse<FilesResponse>(response))
                {
                    tcs.SetResult(null);
                    return;
                }
                Debug.WriteLine("Getting downloaded files and folders");
                tcs.SetResult(response.Data);
            });
            return await tcs.Task;
        }

        // method to move a file or a folder from a source folder to a new destination folder.
        public void MoveFiles(int fileID, int sourceID, int destinationID)
        {
            var request = new WRequest("move/{file id}/from/{source id}/into/{destination id}", Method.GET);

            request.AddUrlSegment("file id", fileID.ToString());
            request.AddUrlSegment("source id", sourceID.ToString());
            request.AddUrlSegment("destination id", destinationID.ToString());

            Debug.WriteLine(request.ToString());
            var asyncHandle = _Client.ExecuteAsync<FolderResponse>(request, response =>
            {
                if (!this.HandleResponse<FolderResponse>(response))
                    return;
                Debug.WriteLine("Moving file or folder");
            });
        }

        // method to get the path/breadcrumb of a file or a folder. ie: All folders from the root to the specified file or folder are listed.

        public async Task<BreadcrumbResponse> BreadcrumbFiles(int ID)
        {
            var tcs = new TaskCompletionSource<BreadcrumbResponse>();

            var request = new WRequest("files/breadcrumb/{file or folder id}", Method.GET);

            request.AddUrlSegment("file or folder id", ID.ToString());

            Debug.WriteLine(request.ToString());
            var asyncHandle = _Client.ExecuteAsync<BreadcrumbResponse>(request, response =>
            {
                if (!this.HandleResponse<BreadcrumbResponse>(response))
                {
                    tcs.SetResult(null);
                    return;
                }
                tcs.SetResult(response.Data);
                Debug.WriteLine("Getting breadcrumb of file or folder");
            });
            return await tcs.Task;
        }
    }
}
