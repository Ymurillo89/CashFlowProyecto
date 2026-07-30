using System.Collections.Generic;
using System.Threading.Tasks;
using AngularApp1.Server.Models;
using AngularApp1.Server.Models.SetModels;
using AngularApp1.Server.Models.ViewModels;

namespace AngularApp1.Server.Services
{
    public interface IBankService
    {
        Task<IEnumerable<GetBank>> GetBanksAsync();
        Task<GetBank?> GetBankByIdAsync(short id);
        Task<Result> PostBankAsync(PostBank model);
        Task<Result> PutBankAsync(short id, PostBank model);
        Task<Result> DeleteBankAsync(short id);
    }
}
