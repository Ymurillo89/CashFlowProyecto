using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using AngularApp1.Server.Data;
using AngularApp1.Server.Models.ViewModels;

namespace AngularApp1.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly DapperContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<object> _passwordHasher;

        public AuthService(DapperContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<object>();
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            // Query that also returns PasswordHash for verification
            var query = @"
                SELECT 
                    u.Id,
                    u.EmpresaId AS CompanyId,
                    e.Nombre AS CompanyName,
                    u.PuntoVentaId AS StoreId,
                    COALESCE(p.Nombre, '') AS StoreName,
                    u.RolId AS RoleId,
                    r.Nombre AS RoleName,
                    u.NombreCompleto AS FullName,
                    u.Email,
                    u.PasswordHash,
                    u.Activo AS IsActive
                FROM Flow_tblUsuarios u
                INNER JOIN Flow_tblEmpresas e ON u.EmpresaId = e.Id
                LEFT JOIN Flow_tblPuntosVenta p ON u.PuntoVentaId = p.Id
                INNER JOIN Flow_tblRoles r ON u.RolId = r.Id
                WHERE LOWER(u.Email) = LOWER(@Email) AND u.Activo = true";

            using var connection = _context.CreateConnection();
            var user = await connection.QuerySingleOrDefaultAsync<UserLoginRecord>(query, new { Email = request.Email });

            if (user == null)
                return null;

            // Verify password using the same PasswordHasher used in UserService
            var result = _passwordHasher.VerifyHashedPassword(new object(), user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return null;

            var token = GenerateJwtToken(user);
            var expiration = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("Jwt:ExpirationHours"));

            return new LoginResponse
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.RoleName,
                CompanyId = user.CompanyId,
                CompanyName = user.CompanyName,
                StoreId = user.StoreId,
                StoreName = user.StoreName,
                ExpiresAt = expiration
            };
        }

        private string GenerateJwtToken(UserLoginRecord user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(jwtSettings.GetValue<int>("ExpirationHours"));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.RoleName),
                new Claim("roleId", user.RoleId.ToString()),
                new Claim("companyId", user.CompanyId.ToString()),
                new Claim("companyName", user.CompanyName),
                new Claim("storeId", user.StoreId?.ToString() ?? ""),
                new Claim("storeName", user.StoreName)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Internal projection class with PasswordHash — NOT exposed outside
        private class UserLoginRecord
        {
            public long Id { get; set; }
            public long CompanyId { get; set; }
            public string CompanyName { get; set; } = string.Empty;
            public long? StoreId { get; set; }
            public string StoreName { get; set; } = string.Empty;
            public int RoleId { get; set; }
            public string RoleName { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
            public bool IsActive { get; set; }
        }
    }
}
