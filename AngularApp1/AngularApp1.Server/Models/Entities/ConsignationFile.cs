using System;

namespace AngularApp1.Server.Models.Entities
{
    public class ConsignationFile
    {
        public long Id { get; set; }
        public long ConsignationId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
