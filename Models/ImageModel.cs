using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestFileUpload.Models
{
    public class ImageModel
    {
        public int ImageId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModified { get; set; }
        public string ExternalKey { get; set; }
        public string tags { get; set; }
    }
}