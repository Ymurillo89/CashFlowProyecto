using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AngularApp1.Server.Models.ViewModels;
using AngularApp1.Server.Models.SetModels;

namespace AngularApp1.Server.Services
{
    public interface IConsignationService
    {
        Task<IEnumerable<GetConsignation>> GetPendingConsignationsAsync();
        Task<IEnumerable<GetConsignation>> GetAllConsignationsAsync();
        Task<GetConsignation> GetConsignationByIdAsync(long id);
        Task<long> SubmitConsignationAsync(PostConsignation request, IFormFile file);
        Task<bool> AuditConsignationAsync(long id, AuditConsignation request);
    }
}
