using System;

namespace AngularApp1.Server.Models.Entities
{
    public class OcrResult
    {
        public long Id { get; set; }
        public long ConsignationId { get; set; }
        public string DetectedBank { get; set; }
        public string DetectedReference { get; set; }
        public decimal? DetectedAmount { get; set; }
        public DateTime? DetectedDate { get; set; }
        public decimal Confidence { get; set; }
        public string RawText { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
