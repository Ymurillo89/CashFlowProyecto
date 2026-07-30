using System.Collections.Generic;
using System.Threading.Tasks;
using AngularApp1.Server.Models;
using AngularApp1.Server.Models.SetModels;
using AngularApp1.Server.Models.ViewModels;

namespace AngularApp1.Server.Services
{
    public interface IUserService
    {
        Task<IEnumerable<GetUser>> GetUsersAsync();
        Task<GetUser?> GetUserByIdAsync(long id);
        Task<Result> PostUserAsync(PostUser model);
        Task<Result> PutUserAsync(long id, PostUser model);
        Task<Result> DeleteUserAsync(long id);
    }
}
