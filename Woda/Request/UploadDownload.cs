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
        // method to delete a file or a folder.
        public async Task<bool> DeleteFile(int fileID)
        {
            var tcs = new TaskCompletionSource<bool>();
            var request = new WRequest("sync/{file id}", Method.DELETE);

            request.AddUrlSegment("file id", fileID.ToString());

            Debug.WriteLine(request.ToString());
            var asyncHandle = _Client.ExecuteAsync<DirectDownloadLink>(request, response =>
            {
                if (!this.HandleResponse<DirectDownloadLink>(response))
                {
                    tcs.SetResult(false);
                    return;
                }
                tcs.SetResult(true);
                Debug.WriteLine("Delete file or folder");
            });
            return await tcs.Task;
        }

        // method to create a new folder at the given path. you can create a whole path. 
        // If you create a whole hierarchy at once the method will just return the last created folder.
        public async Task<FolderResponse> CreateFolder(string folderName)
        {
            var tcs = new TaskCompletionSource<FolderResponse>();
            var request = new WRequest("create_folder", Method.POST);

            request.AddParameter("filename", folderName);

            Debug.WriteLine(request.ToString());
            var asyncHandle = _Client.ExecuteAsync<FolderResponse>(request, response =>
            {
                if (!this.HandleResponse<FolderResponse>(response))
                {
                    tcs.SetResult(null);
                    return;
                }
                tcs.SetResult(response.Data);
                Debug.WriteLine("Create folder");
            });
            return await tcs.Task;
        }

        //method to create a file in database. Call this method to create a file and then upload it.
        public async Task<CreateFileResponse> CreateFile(string filename, string hash, long size)
        {
            var tcs = new TaskCompletionSource<CreateFileResponse>();
            var request = new WRequest("sync", Method.PUT);

            request.AddParameter("filename", filename);
            request.AddParameter("content_hash", hash);
            request.AddParameter("size", size.ToString());

            Debug.WriteLine(request.ToString());
            var asyncHandle = _Client.ExecuteAsync<CreateFileResponse>(request, response =>
            {
                if (!this.HandleResponse<CreateFileResponse>(response))
                {
                    tcs.SetResult(null);
                    return;
                }
                tcs.SetResult(response.Data);
                Debug.WriteLine("Create file");
            });
            return await tcs.Task;
        }

        // Upload file part
        public async Task<UploadFileResponse> UploadFile(int fileID, int partNumber, byte[] data, int n)
        {
            var tcs = new TaskCompletionSource<UploadFileResponse>();
            var request = new WRequest("sync/{file id}/{part number}", Method.PUT);

            request.AddHeader("Content-Type", "application/octet-stream");


            request.AddUrlSegment("file id", fileID.ToString());
            request.AddUrlSegment("part number", partNumber.ToString());
            byte[] buf = new byte[n];

            Array.Copy(data, buf, n);
            string base64String = System.Convert.ToBase64String(data, 0, n);

            string val = Encoding.UTF8.GetString(data, 0, n);

            request.AddParameter("application/octet-stream", buf, ParameterType.RequestBody);
            Debug.WriteLine("size : " + base64String.Length + "  " + data.Length);

            Debug.WriteLine("size : " + val.Length + "  " + data.Length + " " + n + " " + buf.Length);

            var asyncHandle = _Client.ExecuteAsync<UploadFileResponse>(request, response =>
            {
                if (!this.HandleResponse<UploadFileResponse>(response))
                {
                    tcs.SetResult(null);
                    return;
                }
                tcs.SetResult(response.Data);
                Debug.WriteLine("Create file");
            });
            return await tcs.Task;
        }
    }
}
