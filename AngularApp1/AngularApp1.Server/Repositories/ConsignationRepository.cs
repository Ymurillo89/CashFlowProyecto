using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using AngularApp1.Server.Data;
using AngularApp1.Server.Models.Entities;
using AngularApp1.Server.Models.ViewModels;
using System;

namespace AngularApp1.Server.Repositories
{
    public class ConsignationRepository : IConsignationRepository
    {
        private readonly DapperContext _context;

        public ConsignationRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetConsignation>> GetConsignationsByStatusAsync(short statusId)
        {
            var query = @"
                SELECT 
                    c.Id, c.CompanyId, comp.Nombre AS CompanyName, c.StoreId, s.Nombre AS StoreName,
                    c.BankId, b.Nombre AS BankName, c.StatusId,
                    CASE WHEN c.StatusId = 1 THEN 'Pendiente' WHEN c.StatusId = 2 THEN 'Validada' ELSE 'Discrepancia' END AS StatusName,
                    c.ReferenceNumber, c.DeclaredAmount, c.DetectedAmount, c.ConsignationDate, c.ConsignationTime,
                    c.Notes, u1.NombreCompleto AS CreatedByName, u2.NombreCompleto AS ValidatedByName,
                    c.ValidationDate, c.CreatedAt,
                    f.FileUrl,
                    o.Id, o.ConsignationId, o.DetectedBank, o.DetectedReference, o.DetectedAmount, 
                    o.DetectedDate, o.Confidence, o.RawText, o.ProcessedAt
                FROM Flow_tblConsignations c
                INNER JOIN Flow_tblEmpresas comp ON c.CompanyId = comp.Id
                INNER JOIN Flow_tblPuntosVenta s ON c.StoreId = s.Id
                INNER JOIN Flow_tblBancos b ON c.BankId = b.Id
                LEFT JOIN Flow_tblUsuarios u1 ON c.CreatedBy = u1.Id
                LEFT JOIN Flow_tblUsuarios u2 ON c.ValidatedBy = u2.Id
                LEFT JOIN Flow_tblConsignationFiles f ON c.Id = f.ConsignationId
                LEFT JOIN Flow_tblOcrResults o ON c.Id = o.ConsignationId
                WHERE c.StatusId = @StatusId
                ORDER BY c.CreatedAt ASC";

            using (var connection = _context.CreateConnection())
            {
                var dict = new Dictionary<long, GetConsignation>();

                await connection.QueryAsync<GetConsignation, OcrResult, GetConsignation>(
                    query,
                    (consignation, ocr) =>
                    {
                        if (!dict.TryGetValue(consignation.Id, out var currentConsignation))
                        {
                            currentConsignation = consignation;
                            dict.Add(currentConsignation.Id, currentConsignation);
                        }

                        currentConsignation.Ocr = ocr;
                        return currentConsignation;
                    },
                    new { StatusId = statusId },
                    splitOn: "FileUrl,Id"
                );

                return dict.Values;
            }
        }

        public async Task<GetConsignation> GetConsignationByIdAsync(long id)
        {
            var query = @"
                SELECT 
                    c.Id, c.CompanyId, comp.Nombre AS CompanyName, c.StoreId, s.Nombre AS StoreName,
                    c.BankId, b.Nombre AS BankName, c.StatusId,
                    CASE WHEN c.StatusId = 1 THEN 'Pendiente' WHEN c.StatusId = 2 THEN 'Validada' ELSE 'Discrepancia' END AS StatusName,
                    c.ReferenceNumber, c.DeclaredAmount, c.DetectedAmount, c.ConsignationDate, c.ConsignationTime,
                    c.Notes, u1.NombreCompleto AS CreatedByName, u2.NombreCompleto AS ValidatedByName,
                    c.ValidationDate, c.CreatedAt,
                    f.FileUrl,
                    o.Id, o.ConsignationId, o.DetectedBank, o.DetectedReference, o.DetectedAmount, 
                    o.DetectedDate, o.Confidence, o.RawText, o.ProcessedAt
                FROM Flow_tblConsignations c
                INNER JOIN Flow_tblEmpresas comp ON c.CompanyId = comp.Id
                INNER JOIN Flow_tblPuntosVenta s ON c.StoreId = s.Id
                INNER JOIN Flow_tblBancos b ON c.BankId = b.Id
                LEFT JOIN Flow_tblUsuarios u1 ON c.CreatedBy = u1.Id
                LEFT JOIN Flow_tblUsuarios u2 ON c.ValidatedBy = u2.Id
                LEFT JOIN Flow_tblConsignationFiles f ON c.Id = f.ConsignationId
                LEFT JOIN Flow_tblOcrResults o ON c.Id = o.ConsignationId
                WHERE c.Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var dict = new Dictionary<long, GetConsignation>();

                await connection.QueryAsync<GetConsignation, OcrResult, GetConsignation>(
                    query,
                    (consignation, ocr) =>
                    {
                        if (!dict.TryGetValue(consignation.Id, out var currentConsignation))
                        {
                            currentConsignation = consignation;
                            dict.Add(currentConsignation.Id, currentConsignation);
                        }

                        currentConsignation.Ocr = ocr;
                        return currentConsignation;
                    },
                    new { Id = id },
                    splitOn: "FileUrl,Id"
                );

                return dict.Values.FirstOrDefault();
            }
        }

        public async Task<long> CreateConsignationAsync(Consignation c, ConsignationFile f, OcrResult o)
        {
            using (var connection = _context.CreateConnection())
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var insertConsignation = @"
                            INSERT INTO Flow_tblConsignations 
                            (CompanyId, StoreId, BankId, StatusId, ReferenceNumber, DeclaredAmount, DetectedAmount, 
                             ConsignationDate, ConsignationTime, Notes, CreatedBy, CreatedAt)
                            VALUES 
                            (@CompanyId, @StoreId, @BankId, @StatusId, @ReferenceNumber, @DeclaredAmount, @DetectedAmount,
                             @ConsignationDate, @ConsignationTime, @Notes, @CreatedBy, @CreatedAt)
                            RETURNING Id;";

                        var id = await connection.ExecuteScalarAsync<long>(insertConsignation, c, transaction);
                        c.Id = id;

                        var insertFile = @"
                            INSERT INTO Flow_tblConsignationFiles 
                            (ConsignationId, FileName, FileUrl, FileType, FileSize, UploadedAt)
                            VALUES 
                            (@ConsignationId, @FileName, @FileUrl, @FileType, @FileSize, @UploadedAt);";
                        
                        f.ConsignationId = id;
                        await connection.ExecuteAsync(insertFile, f, transaction);

                        var insertOcr = @"
                            INSERT INTO Flow_tblOcrResults 
                            (ConsignationId, DetectedBank, DetectedReference, DetectedAmount, DetectedDate, 
                             Confidence, RawText, ProcessedAt)
                            VALUES 
                            (@ConsignationId, @DetectedBank, @DetectedReference, @DetectedAmount, @DetectedDate,
                             @Confidence, @RawText, @ProcessedAt);";
                        
                        o.ConsignationId = id;
                        await connection.ExecuteAsync(insertOcr, o, transaction);

                        transaction.Commit();
                        return id;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> UpdateStatusAsync(long id, short statusId, long validatorId, string comments)
        {
            var query = @"
                UPDATE Flow_tblConsignations
                SET StatusId = @StatusId, 
                    ValidatedBy = @ValidatorId, 
                    ValidationDate = @ValidationDate, 
                    Notes = COALESCE(Notes, '') || CHR(10) || @Comments
                WHERE Id = @Id;";

            using (var connection = _context.CreateConnection())
            {
                var affected = await connection.ExecuteAsync(query, new { Id = id, StatusId = statusId, ValidatorId = validatorId, ValidationDate = DateTime.Now, Comments = comments });
                return affected > 0;
            }
        }
    }
}
