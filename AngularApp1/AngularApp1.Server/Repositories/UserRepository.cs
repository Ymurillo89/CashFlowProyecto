using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using AngularApp1.Server.Data;
using AngularApp1.Server.Models;
using AngularApp1.Server.Models.SetModels;
using AngularApp1.Server.Models.ViewModels;

namespace AngularApp1.Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetUser>> GetAllUsersAsync()
        {
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
                    u.Activo AS IsActive, 
                    u.FechaCreacion AS CreatedAt 
                FROM Flow_tblUsuarios u
                INNER JOIN Flow_tblEmpresas e ON u.EmpresaId = e.Id
                LEFT JOIN Flow_tblPuntosVenta p ON u.PuntoVentaId = p.Id
                INNER JOIN Flow_tblRoles r ON u.RolId = r.Id
                ORDER BY u.Id DESC";

            using var connection = _context.CreateConnection();
            var users = (await connection.QueryAsync<GetUser>(query)).ToList();

            var associations = await connection.QueryAsync<(long UsuarioId, long PuntoVentaId)>(
                "SELECT UsuarioId, PuntoVentaId FROM Flow_tblUsuarioPuntosVenta");
            var lookup = associations.ToLookup(a => a.UsuarioId, a => a.PuntoVentaId);

            foreach (var user in users)
            {
                user.AssignedStoreIds = lookup[user.Id].ToList();
            }

            return users;
        }

        public async Task<GetUser?> GetUserByIdAsync(long id)
        {
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
                    u.Activo AS IsActive, 
                    u.FechaCreacion AS CreatedAt 
                FROM Flow_tblUsuarios u
                INNER JOIN Flow_tblEmpresas e ON u.EmpresaId = e.Id
                LEFT JOIN Flow_tblPuntosVenta p ON u.PuntoVentaId = p.Id
                INNER JOIN Flow_tblRoles r ON u.RolId = r.Id
                WHERE u.Id = @Id";

            using var connection = _context.CreateConnection();
            var user = await connection.QuerySingleOrDefaultAsync<GetUser>(query, new { Id = id });
            if (user != null)
            {
                var storeIds = await connection.QueryAsync<long>(
                    "SELECT PuntoVentaId FROM Flow_tblUsuarioPuntosVenta WHERE UsuarioId = @Id", new { Id = user.Id });
                user.AssignedStoreIds = storeIds.ToList();
            }
            return user;
        }

        public async Task<GetUser?> GetUserByEmailAsync(string email)
        {
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
                    u.Activo AS IsActive, 
                    u.FechaCreacion AS CreatedAt 
                FROM Flow_tblUsuarios u
                INNER JOIN Flow_tblEmpresas e ON u.EmpresaId = e.Id
                LEFT JOIN Flow_tblPuntosVenta p ON u.PuntoVentaId = p.Id
                INNER JOIN Flow_tblRoles r ON u.RolId = r.Id
                WHERE LOWER(u.Email) = LOWER(@Email)";

            using var connection = _context.CreateConnection();
            var user = await connection.QuerySingleOrDefaultAsync<GetUser>(query, new { Email = email });
            if (user != null)
            {
                var storeIds = await connection.QueryAsync<long>(
                    "SELECT PuntoVentaId FROM Flow_tblUsuarioPuntosVenta WHERE UsuarioId = @Id", new { Id = user.Id });
                user.AssignedStoreIds = storeIds.ToList();
            }
            return user;
        }

        public async Task<Result> CreateUserAsync(PostUser model, string hashedPassword)
        {
            var query = @"
                INSERT INTO Flow_tblUsuarios (EmpresaId, PuntoVentaId, RolId, NombreCompleto, Email, PasswordHash, Activo) 
                VALUES (@CompanyId, @StoreId, @RoleId, @FullName, @Email, @PasswordHash, @IsActive)
                RETURNING Id;";

            var primaryStoreId = model.AssignedStoreIds != null && model.AssignedStoreIds.Any() ? (long?)model.AssignedStoreIds.First() : model.StoreId;

            var parameters = new DynamicParameters();
            parameters.Add("CompanyId", model.CompanyId);
            parameters.Add("StoreId", primaryStoreId);
            parameters.Add("RoleId", model.RoleId);
            parameters.Add("FullName", model.FullName);
            parameters.Add("Email", model.Email);
            parameters.Add("PasswordHash", hashedPassword);
            parameters.Add("IsActive", model.IsActive);

            try
            {
                using var connection = _context.CreateConnection();
                var newUserId = await connection.ExecuteScalarAsync<long>(query, parameters);

                if (newUserId > 0)
                {
                    if (model.AssignedStoreIds != null && model.AssignedStoreIds.Any())
                    {
                        foreach (var sId in model.AssignedStoreIds)
                        {
                            await connection.ExecuteAsync(
                                "INSERT INTO Flow_tblUsuarioPuntosVenta (UsuarioId, PuntoVentaId) VALUES (@UserId, @StoreId) ON CONFLICT DO NOTHING",
                                new { UserId = newUserId, StoreId = sId });
                        }
                    }
                    return new Result { Success = true, Message = "User created successfully" };
                }
                return new Result { Success = false, Message = "Could not create user" };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<Result> UpdateUserAsync(long id, PostUser model, string? hashedPassword)
        {
            string query;
            var primaryStoreId = model.AssignedStoreIds != null && model.AssignedStoreIds.Any() ? (long?)model.AssignedStoreIds.First() : model.StoreId;

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("CompanyId", model.CompanyId);
            parameters.Add("StoreId", primaryStoreId);
            parameters.Add("RoleId", model.RoleId);
            parameters.Add("FullName", model.FullName);
            parameters.Add("Email", model.Email);
            parameters.Add("IsActive", model.IsActive);

            if (!string.IsNullOrWhiteSpace(hashedPassword))
            {
                query = @"
                    UPDATE Flow_tblUsuarios 
                    SET EmpresaId = @CompanyId, 
                        PuntoVentaId = @StoreId, 
                        RolId = @RoleId, 
                        NombreCompleto = @FullName, 
                        Email = @Email, 
                        PasswordHash = @PasswordHash, 
                        Activo = @IsActive 
                    WHERE Id = @Id";
                parameters.Add("PasswordHash", hashedPassword);
            }
            else
            {
                query = @"
                    UPDATE Flow_tblUsuarios 
                    SET EmpresaId = @CompanyId, 
                        PuntoVentaId = @StoreId, 
                        RolId = @RoleId, 
                        NombreCompleto = @FullName, 
                        Email = @Email, 
                        Activo = @IsActive 
                    WHERE Id = @Id";
            }

            try
            {
                using var connection = _context.CreateConnection();
                var rowsAffected = await connection.ExecuteAsync(query, parameters);

                if (rowsAffected > 0)
                {
                    // Update store associations
                    await connection.ExecuteAsync("DELETE FROM Flow_tblUsuarioPuntosVenta WHERE UsuarioId = @Id", new { Id = id });
                    if (model.AssignedStoreIds != null && model.AssignedStoreIds.Any())
                    {
                        foreach (var sId in model.AssignedStoreIds)
                        {
                            await connection.ExecuteAsync(
                                "INSERT INTO Flow_tblUsuarioPuntosVenta (UsuarioId, PuntoVentaId) VALUES (@UserId, @StoreId) ON CONFLICT DO NOTHING",
                                new { UserId = id, StoreId = sId });
                        }
                    }
                    return new Result { Success = true, Message = "User updated successfully" };
                }
                return new Result { Success = false, Message = "User not found or no changes made" };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<Result> DeleteUserAsync(long id)
        {
            var query = "DELETE FROM Flow_tblUsuarios WHERE Id = @Id";

            try
            {
                using var connection = _context.CreateConnection();
                var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });

                if (rowsAffected > 0)
                {
                    return new Result { Success = true, Message = "User deleted successfully" };
                }
                return new Result { Success = false, Message = "User not found" };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Error: {ex.Message}" };
            }
        }
    }
}
