using System;

namespace AngularApp1.Server.Models.SetModels
{
    public class PostConsignation
    {
        public long StoreId { get; set; }
        public long BankId { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal DeclaredAmount { get; set; }
        public DateTime ConsignationDate { get; set; }
        public string ConsignationTime { get; set; }
        public string Notes { get; set; }
    }
}
