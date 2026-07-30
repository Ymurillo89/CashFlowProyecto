using System;

namespace AngularApp1.Server.Models.ViewModels
{
    public class GetUser
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public long? StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public short RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
