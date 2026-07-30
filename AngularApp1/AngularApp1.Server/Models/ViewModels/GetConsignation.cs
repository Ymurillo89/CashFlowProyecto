using System;
using AngularApp1.Server.Models.Entities;

namespace AngularApp1.Server.Models.ViewModels
{
    public class GetConsignation
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public long StoreId { get; set; }
        public string StoreName { get; set; }
        public long BankId { get; set; }
        public string BankName { get; set; }
        public short StatusId { get; set; }
        public string StatusName { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal DeclaredAmount { get; set; }
        public decimal? DetectedAmount { get; set; }
        public DateTime? ConsignationDate { get; set; }
        public TimeSpan? ConsignationTime { get; set; }
        public string Notes { get; set; }
        public string CreatedByName { get; set; }
        public string ValidatedByName { get; set; }
        public DateTime? ValidationDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FileUrl { get; set; }
        public OcrResult Ocr { get; set; }
    }
}
