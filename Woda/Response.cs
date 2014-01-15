using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woda
{
    public class Response
    {
        public Response() { }
        public string error { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
    }

    public class UserResponse : Response
    {
        public UserResponse() { }
        public User user { get; set; }
    }

    public class FolderResponse : Response
    {
        public FolderResponse() { }
        public Folder folder { get; set; }
    }

    public class FilesResponse : Response
    {
        public FilesResponse() { }
        public List<File> files { get; set; }
    }

    public class FileResponse : Response
    {
        public FileResponse() { }
        public File file { get; set; }
    }

    public class BreadcrumbResponse : Response
    {
        public BreadcrumbResponse() { }
        public List<Folder> breadcrumb { get; set; }
    }

    public class CreateFileResponse : Response
    {
        public CreateFileResponse() { }
        public bool uploaded { get; set; }
        public List<int> needed_parts { get; set; }
        public int part_size { get; set; }
        public File file { get; set; }
    }

    public class UploadFileResponse : Response
    {
        public UploadFileResponse() { }
        public bool uploaded { get; set; }
        public List<int> needed_parts { get; set; }
    }

    public class DirectDownloadLink : Response
    {
        public DirectDownloadLink() { }
        public string link { get; set; }
        public string uuid { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string login { get; set; }
        public string email { get; set; }
        public bool active { get; set; }
        public bool locked { get; set; }
        public bool admin { get; set; }
        public int space { get; set; }
        public int used_space { get; set; }
    }

    public class GenericFile
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool @public { get; set; }
        public string last_update { get; set; }
        public bool favorite { get; set; }
        public bool folder { get; set; }
        public bool shared { get; set; }
        public int downloads { get; set; }
    }

    public class File : GenericFile
    {
        public string type { get; set; }
        public int size { get; set; }
        public int part_size { get; set; }
        public bool uploaded { get; set; }
        public string link { get; set; }
        public string uuid { get; set; }
    }

    public class Folder : GenericFile
    {
        public List<Folder> folders { get; set; }
        public List<File> files { get; set; }
    }
}
