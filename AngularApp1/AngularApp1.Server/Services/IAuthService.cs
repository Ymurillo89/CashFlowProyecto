using AngularApp1.Server.Models.ViewModels;

namespace AngularApp1.Server.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
