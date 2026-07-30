using System;

namespace AngularApp1.Server.Models.Entities
{
    public class Store
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public string ManagerPhone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
