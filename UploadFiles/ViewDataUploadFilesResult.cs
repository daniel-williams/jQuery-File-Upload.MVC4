using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UploadFiles
{
    public class ViewDataUploadFilesResult
    {
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }
    }
}