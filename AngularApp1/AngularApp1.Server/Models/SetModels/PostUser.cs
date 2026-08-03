namespace AngularApp1.Server.Models.SetModels
{
    public class PostUser
    {
        public long CompanyId { get; set; }
        public long? StoreId { get; set; }
        public short RoleId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public System.Collections.Generic.List<long> AssignedStoreIds { get; set; } = new();
    }
}
