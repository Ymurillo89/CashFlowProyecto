using System.Threading.Tasks;
using AngularApp1.Server.Models.Entities;

namespace AngularApp1.Server.Services
{
    public interface IOcrService
    {
        Task<OcrResult> ProcessImageAsync(string filePath, string declaredReference, decimal declaredAmount);
    }
}
