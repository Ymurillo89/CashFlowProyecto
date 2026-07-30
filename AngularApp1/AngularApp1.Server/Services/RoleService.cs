using System.Collections.Generic;
using System.Threading.Tasks;
using AngularApp1.Server.Models.ViewModels;
using AngularApp1.Server.Repositories;

namespace AngularApp1.Server.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;

        public RoleService(IRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<GetRole>> GetRolesAsync()
        {
            return await _repository.GetAllRolesAsync();
        }
    }
}
