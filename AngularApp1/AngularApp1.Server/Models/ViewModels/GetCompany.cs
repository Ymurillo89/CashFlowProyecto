using System;

namespace AngularApp1.Server.Models.ViewModels
{
    public class GetCompany
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
