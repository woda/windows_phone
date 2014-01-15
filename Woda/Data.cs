using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woda
{

    public sealed class Data
    {
        public User _User { get; set; }
        public Folder _CurrentFolder { get; set; }
        public List<int> _NavigationFoldersIDs { get; set; }
        public Cookie _SessionCookie { get; set; }
        public static Dictionary<string, string> _Types { get; set; }
        public static Dictionary<string, string> _FileFormat { get; set; }

        private static volatile Data instance;
        private static object syncRoot = new Object();

        private Data()
        {
            _NavigationFoldersIDs = new List<int>();

            _FileFormat = new Dictionary<string, string>()
            {
                { @"image", "/Resources/Images/list_icon_picture.png"},
                { @"video", "/Resources/Images/list_icon_movie.png"},
                { @"music", "/Resources/Images/list_icon_music.png"},
                { @"folder", "/Resources/Images/list_icon_folder.png"},
                { @"default", "/Resources/Images/list_icon_document.png"}
            };

            _Types = new Dictionary<string, string>()
            {
                { @".JPG", _FileFormat["image"]},
                { @".JPEG", _FileFormat["image"]},
                { @".PNG", _FileFormat["image"]},
                { @".TIFF", _FileFormat["image"]},
                { @".GIF", _FileFormat["image"]},
                { @".RAW", _FileFormat["image"]},
                { @".BMP", _FileFormat["image"]},

                { @".MOV", _FileFormat["video"]},
                { @".MPG", _FileFormat["video"]},
                { @".M4V", _FileFormat["video"]},
                { @".MKV", _FileFormat["video"]},
                { @".M2TS", _FileFormat["video"]},
                { @".WMV", _FileFormat["video"]},

                { @".MP3", _FileFormat["music"]},
                { @".WAV", _FileFormat["music"]},
                { @".FLAC", _FileFormat["music"]},
                { @".AIFF", _FileFormat["music"]},
                { @".WMA", _FileFormat["music"]}

            };
        }

        public static Data Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Data();
                    }
                }
                return instance;
            }

        }

        public class File
        {
            public string _Name { get; set; }
            public string _Icon { get; set; }
            public string _SizeAndDate { get; set; }
            public object _File { get; set; }
            public string _FavoriteIcon { get; set; }
            public string _PublicIcon { get; set; }
        }

    }
}
