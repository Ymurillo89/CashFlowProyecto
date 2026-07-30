using AngularApp1.Server.Models.Entities;
using AngularApp1.Server.Data;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AngularApp1.Server.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly DapperContext _context;

        public StoreRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Store>> GetAllAsync()
        {
            var query = @"
                SELECT 
                    Id,
                    EmpresaId AS CompanyId,
                    Codigo AS Code,
                    Nombre AS Name,
                    Ciudad AS City,
                    Direccion AS Address,
                    NombreGerente AS ManagerName,
                    TelefonoGerente AS ManagerPhone,
                    Activo AS IsActive,
                    FechaCreacion AS CreatedAt
                FROM Flow_tblPuntosVenta";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Store>(query);
        }

        public async Task<Store?> GetByIdAsync(long id)
        {
            var query = @"
                SELECT 
                    Id,
                    EmpresaId AS CompanyId,
                    Codigo AS Code,
                    Nombre AS Name,
                    Ciudad AS City,
                    Direccion AS Address,
                    NombreGerente AS ManagerName,
                    TelefonoGerente AS ManagerPhone,
                    Activo AS IsActive,
                    FechaCreacion AS CreatedAt
                FROM Flow_tblPuntosVenta
                WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Store>(query, new { Id = id });
        }

        public async Task<long> CreateAsync(Store store)
        {
            var query = @"
                INSERT INTO Flow_tblPuntosVenta (EmpresaId, Codigo, Nombre, Ciudad, Direccion, NombreGerente, TelefonoGerente, Activo)
                VALUES (@CompanyId, @Code, @Name, @City, @Address, @ManagerName, @ManagerPhone, @IsActive)
                RETURNING Id";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<long>(query, store);
        }

        public async Task<bool> UpdateAsync(Store store)
        {
            var query = @"
                UPDATE Flow_tblPuntosVenta
                SET EmpresaId = @CompanyId,
                    Codigo = @Code,
                    Nombre = @Name,
                    Ciudad = @City,
                    Direccion = @Address,
                    NombreGerente = @ManagerName,
                    TelefonoGerente = @ManagerPhone,
                    Activo = @IsActive
                WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteAsync(query, store);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var query = "DELETE FROM Flow_tblPuntosVenta WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteAsync(query, new { Id = id });
            return result > 0;
        }
    }
}
