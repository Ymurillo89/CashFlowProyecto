using AngularApp1.Server.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AngularApp1.Server.Services
{
    public interface IStoreService
    {
        Task<IEnumerable<Store>> GetAllStoresAsync();
        Task<Store?> GetStoreByIdAsync(long id);
        Task<Store> CreateStoreAsync(Store store);
        Task<bool> UpdateStoreAsync(long id, Store store);
        Task<bool> DeleteStoreAsync(long id);
    }
}
