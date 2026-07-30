using System;

namespace AngularApp1.Server.Models.Entities
{
    public class User
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public long? StoreId { get; set; }
        public short RoleId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
