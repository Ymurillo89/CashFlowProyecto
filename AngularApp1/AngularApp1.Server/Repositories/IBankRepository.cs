using System.Collections.Generic;
using System.Threading.Tasks;
using AngularApp1.Server.Models;
using AngularApp1.Server.Models.SetModels;
using AngularApp1.Server.Models.ViewModels;

namespace AngularApp1.Server.Repositories
{
    public interface IBankRepository
    {
        Task<IEnumerable<GetBank>> GetAllBanksAsync();
        Task<GetBank?> GetBankByIdAsync(short id);
        Task<Result> CreateBankAsync(PostBank model);
        Task<Result> UpdateBankAsync(short id, PostBank model);
        Task<Result> DeleteBankAsync(short id);
    }
}
