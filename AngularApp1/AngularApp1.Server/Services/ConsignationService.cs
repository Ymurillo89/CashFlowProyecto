using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using AngularApp1.Server.Models.Entities;
using AngularApp1.Server.Models.SetModels;
using AngularApp1.Server.Models.ViewModels;
using AngularApp1.Server.Repositories;

namespace AngularApp1.Server.Services
{
    public class ConsignationService : IConsignationService
    {
        private readonly IConsignationRepository _repository;
        private readonly IOcrService _ocrService;
        private readonly IWebHostEnvironment _env;

        public ConsignationService(IConsignationRepository repository, IOcrService ocrService, IWebHostEnvironment env)
        {
            _repository = repository;
            _ocrService = ocrService;
            _env = env;
        }

        public async Task<IEnumerable<GetConsignation>> GetPendingConsignationsAsync()
        {
            return await _repository.GetConsignationsByStatusAsync(1); // 1 = Pendiente
        }

        public async Task<GetConsignation> GetConsignationByIdAsync(long id)
        {
            return await _repository.GetConsignationByIdAsync(id);
        }

        public async Task<long> SubmitConsignationAsync(PostConsignation request, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("El archivo es obligatorio");

            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var ocrResult = await _ocrService.ProcessImageAsync(filePath, request.ReferenceNumber, request.DeclaredAmount);

            // Fetch companyId from store? Hardcoded to 1 for now
            var consignation = new Consignation
            {
                CompanyId = 1,
                StoreId = request.StoreId,
                BankId = request.BankId,
                StatusId = 1, // Pendiente
                ReferenceNumber = request.ReferenceNumber,
                DeclaredAmount = request.DeclaredAmount,
                DetectedAmount = ocrResult.DetectedAmount,
                ConsignationDate = request.ConsignationDate,
                ConsignationTime = string.IsNullOrEmpty(request.ConsignationTime) ? null : TimeSpan.Parse(request.ConsignationTime),
                Notes = request.Notes,
                CreatedBy = 1, 
                CreatedAt = DateTime.Now
            };

            var consignationFile = new ConsignationFile
            {
                FileName = file.FileName,
                FileUrl = $"/uploads/{fileName}",
                FileType = file.ContentType,
                FileSize = file.Length,
                UploadedAt = DateTime.Now
            };

            return await _repository.CreateConsignationAsync(consignation, consignationFile, ocrResult);
        }

        public async Task<bool> AuditConsignationAsync(long id, AuditConsignation request)
        {
            long validatorId = 1; 
            return await _repository.UpdateStatusAsync(id, request.StatusId, validatorId, request.Comments);
        }
    }
}
