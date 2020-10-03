using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Bridge.Models.ApiModels
{

    public class GraphFilesModel
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        [JsonProperty("@odata.nextLink")]
        public string OdataNextLink { get; set; }
        [JsonProperty("value")]
        public List<FileItemModel> FileItems { get; set; }
    }

    public class FileItemModel
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public Folder folder { get; set; }
        public File file { get; set; }
        [JsonProperty("thumbnails@odata.context")]
        public string OdataThumbnailsContext { get; set; }
        public List<Thumbnail> thumbnails { get; set; }

        public override string ToString()
        {
            return name;
        }
    }

    public class Folder
    {
        public int childCount { get; set; }
    }

    public class Thumbnail
    {
        [JsonProperty("c200x200_x005f_crop")]
        public thumbProps thumb { get; set; }
    }

    public class thumbProps
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class File
    {
        public string mimeType { get; set; }
        public Hashes hashes { get; set; }
    }

    public class Hashes
    {
        public string quickXorHash { get; set; }
    }
}
