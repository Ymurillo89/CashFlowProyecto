using System.Collections.Generic;
using System.Threading.Tasks;
using AngularApp1.Server.Models.ViewModels;
using AngularApp1.Server.Models.Entities;

namespace AngularApp1.Server.Repositories
{
    public interface IConsignationRepository
    {
        Task<IEnumerable<GetConsignation>> GetConsignationsByStatusAsync(short statusId, long? userId = null, string? roleName = null);
        Task<IEnumerable<GetConsignation>> GetAllConsignationsAsync(long? userId = null, string? roleName = null);
        Task<GetConsignation> GetConsignationByIdAsync(long id);
        Task<long> CreateConsignationAsync(Consignation consignation, ConsignationFile file, OcrResult ocrResult);
        Task<bool> UpdateStatusAsync(long id, short statusId, long validatorId, string comments);
    }
}
