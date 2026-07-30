using AngularApp1.Server.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AngularApp1.Server.Repositories
{
    public interface IStoreRepository
    {
        Task<IEnumerable<Store>> GetAllAsync();
        Task<Store?> GetByIdAsync(long id);
        Task<long> CreateAsync(Store store);
        Task<bool> UpdateAsync(Store store);
        Task<bool> DeleteAsync(long id);
    }
}
