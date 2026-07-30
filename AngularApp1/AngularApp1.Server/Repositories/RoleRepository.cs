using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using AngularApp1.Server.Data;
using AngularApp1.Server.Models.ViewModels;

namespace AngularApp1.Server.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DapperContext _context;

        public RoleRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetRole>> GetAllRolesAsync()
        {
            var query = "SELECT Id, Nombre AS Name FROM Flow_tblRoles ORDER BY Id ASC";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<GetRole>(query);
        }
    }
}
