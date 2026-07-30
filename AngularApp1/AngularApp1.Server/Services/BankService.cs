using System.Collections.Generic;
using System.Threading.Tasks;
using AngularApp1.Server.Models;
using AngularApp1.Server.Models.SetModels;
using AngularApp1.Server.Models.ViewModels;
using AngularApp1.Server.Repositories;

namespace AngularApp1.Server.Services
{
    public class BankService : IBankService
    {
        private readonly IBankRepository _repository;

        public BankService(IBankRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<GetBank>> GetBanksAsync()
        {
            return await _repository.GetAllBanksAsync();
        }

        public async Task<GetBank?> GetBankByIdAsync(short id)
        {
            return await _repository.GetBankByIdAsync(id);
        }

        public async Task<Result> PostBankAsync(PostBank model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return new Result { Success = false, Message = "Bank name is required." };
            }

            return await _repository.CreateBankAsync(model);
        }

        public async Task<Result> PutBankAsync(short id, PostBank model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return new Result { Success = false, Message = "Bank name is required." };
            }

            return await _repository.UpdateBankAsync(id, model);
        }

        public async Task<Result> DeleteBankAsync(short id)
        {
            return await _repository.DeleteBankAsync(id);
        }
    }
}
