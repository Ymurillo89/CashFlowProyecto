using System.Collections.Generic;
using System.Threading.Tasks;
using AngularApp1.Server.Models.ViewModels;

namespace AngularApp1.Server.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<GetRole>> GetRolesAsync();
    }
}
