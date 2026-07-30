using System;

namespace AngularApp1.Server.Models.Entities
{
    public class Consignation
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public long StoreId { get; set; }
        public long BankId { get; set; }
        public short StatusId { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal DeclaredAmount { get; set; }
        public decimal? DetectedAmount { get; set; }
        public DateTime? ConsignationDate { get; set; }
        public TimeSpan? ConsignationTime { get; set; }
        public string Notes { get; set; }
        public long? CreatedBy { get; set; }
        public long? ValidatedBy { get; set; }
        public DateTime? ValidationDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
