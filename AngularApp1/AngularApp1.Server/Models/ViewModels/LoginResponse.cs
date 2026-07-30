namespace AngularApp1.Server.Models.ViewModels
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public long CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public long? StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
