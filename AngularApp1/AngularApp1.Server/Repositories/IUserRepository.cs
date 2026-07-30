using System.Collections.Generic;
using System.Threading.Tasks;
using AngularApp1.Server.Models;
using AngularApp1.Server.Models.SetModels;
using AngularApp1.Server.Models.ViewModels;

namespace AngularApp1.Server.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<GetUser>> GetAllUsersAsync();
        Task<GetUser?> GetUserByIdAsync(long id);
        Task<GetUser?> GetUserByEmailAsync(string email);
        Task<Result> CreateUserAsync(PostUser model, string hashedPassword);
        Task<Result> UpdateUserAsync(long id, PostUser model, string? hashedPassword);
        Task<Result> DeleteUserAsync(long id);
    }
}
