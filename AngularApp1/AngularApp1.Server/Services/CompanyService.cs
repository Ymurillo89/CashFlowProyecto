using System.Collections.Generic;
using System.Threading.Tasks;
using AngularApp1.Server.Models;
using AngularApp1.Server.Models.SetModels;
using AngularApp1.Server.Models.ViewModels;
using AngularApp1.Server.Repositories;

namespace AngularApp1.Server.Services
{
    public class CompanyService
    {
        private readonly CompanyRepository _repository;

        public CompanyService(CompanyRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<GetCompany>> GetCompaniesAsync()
        {
            return await _repository.GetAllCompaniesAsync();
        }

        public async Task<GetCompany?> GetCompanyByIdAsync(long id)
        {
            return await _repository.GetCompanyByIdAsync(id);
        }

        public async Task<Result> PostCompanyAsync(PostCompany model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return new Result { Success = false, Message = "Company name is required." };
            }

            return await _repository.CreateCompanyAsync(model);
        }

        public async Task<Result> UpdateCompanyAsync(long id, PostCompany model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return new Result { Success = false, Message = "Company name is required." };
            }

            return await _repository.UpdateCompanyAsync(id, model);
        }

        public async Task<Result> DeleteCompanyAsync(long id)
        {
            return await _repository.DeleteCompanyAsync(id);
        }
    }
}
