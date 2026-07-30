using AngularApp1.Server.Models.Entities;
using AngularApp1.Server.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AngularApp1.Server.Services
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;

        public StoreService(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<IEnumerable<Store>> GetAllStoresAsync()
        {
            return await _storeRepository.GetAllAsync();
        }

        public async Task<Store?> GetStoreByIdAsync(long id)
        {
            return await _storeRepository.GetByIdAsync(id);
        }

        public async Task<Store> CreateStoreAsync(Store store)
        {
            var id = await _storeRepository.CreateAsync(store);
            store.Id = id;
            return store;
        }

        public async Task<bool> UpdateStoreAsync(long id, Store store)
        {
            store.Id = id;
            return await _storeRepository.UpdateAsync(store);
        }

        public async Task<bool> DeleteStoreAsync(long id)
        {
            return await _storeRepository.DeleteAsync(id);
        }
    }
}
