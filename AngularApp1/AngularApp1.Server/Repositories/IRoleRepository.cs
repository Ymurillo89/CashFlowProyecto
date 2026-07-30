using System.Collections.Generic;
using System.Threading.Tasks;
using AngularApp1.Server.Models.ViewModels;

namespace AngularApp1.Server.Repositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<GetRole>> GetAllRolesAsync();
    }
}
