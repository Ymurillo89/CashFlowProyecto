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

        public async Task<IEnumerable<GetConsignation>> GetConsignationsByStatusAsync(short statusId, long? userId = null, string? roleName = null)
        {
            var isAdmin = roleName == "Administrador";
            var query = @"
                SELECT 
                    c.Id, c.EmpresaId AS CompanyId, comp.Nombre AS CompanyName, c.PuntoVentaId AS StoreId,
                    s.Nombre AS StoreName, c.BancoId AS BankId, b.Nombre AS BankName, c.EstadoId AS StatusId,
                    est.Nombre AS StatusName,
                    c.NumeroReferencia AS ReferenceNumber, c.MontoDeclarado AS DeclaredAmount, c.MontoDetectado AS DetectedAmount,
                    c.FechaConsignacion::timestamp AS ConsignationDate, c.HoraConsignacion::interval AS ConsignationTime,
                    c.Observaciones AS Notes, u1.NombreCompleto AS CreatedByName, u2.NombreCompleto AS ValidatedByName,
                    c.FechaValidacion AS ValidationDate, c.FechaCreacion AS CreatedAt,
                    f.UrlArchivo AS FileUrl,
                    o.Id, o.ConsignacionId AS ConsignationId, o.BancoDetectado AS DetectedBank, o.ReferenciaDetectada AS DetectedReference,
                    o.MontoDetectado AS DetectedAmount, o.FechaDetectada::timestamp AS DetectedDate,
                    o.Confianza AS Confidence, o.TextoCrudo AS RawText, o.FechaProcesamiento AS ProcessedAt
                FROM Flow_tblConsignaciones c
                INNER JOIN Flow_tblEmpresas comp ON c.EmpresaId = comp.Id
                INNER JOIN Flow_tblPuntosVenta s ON c.PuntoVentaId = s.Id
                INNER JOIN Flow_tblBancos b ON c.BancoId = b.Id
                INNER JOIN Flow_tblEstadosConsignacion est ON c.EstadoId = est.Id
                LEFT JOIN Flow_tblUsuarios u1 ON c.CreadoPor = u1.Id
                LEFT JOIN Flow_tblUsuarios u2 ON c.ValidadoPor = u2.Id
                LEFT JOIN Flow_tblArchivosConsignacion f ON c.Id = f.ConsignacionId
                LEFT JOIN Flow_tblResultadosOcr o ON c.Id = o.ConsignacionId
                WHERE c.EstadoId = @StatusId
                  AND (@IsAdmin = TRUE OR c.PuntoVentaId IN (
                      SELECT PuntoVentaId FROM Flow_tblUsuarioPuntosVenta WHERE UsuarioId = @UserId
                  ))
                ORDER BY c.FechaCreacion DESC";

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
                    new { StatusId = statusId, UserId = userId, IsAdmin = isAdmin },
                    splitOn: "Id"
                );

                return dict.Values;
            }
        }

        public async Task<IEnumerable<GetConsignation>> GetAllConsignationsAsync(long? userId = null, string? roleName = null)
        {
            var isAdmin = roleName == "Administrador";
            var query = @"
                SELECT 
                    c.Id, c.EmpresaId AS CompanyId, comp.Nombre AS CompanyName, c.PuntoVentaId AS StoreId,
                    s.Nombre AS StoreName, c.BancoId AS BankId, b.Nombre AS BankName, c.EstadoId AS StatusId,
                    est.Nombre AS StatusName,
                    c.NumeroReferencia AS ReferenceNumber, c.MontoDeclarado AS DeclaredAmount, c.MontoDetectado AS DetectedAmount,
                    c.FechaConsignacion::timestamp AS ConsignationDate, c.HoraConsignacion::interval AS ConsignationTime,
                    c.Observaciones AS Notes, u1.NombreCompleto AS CreatedByName, u2.NombreCompleto AS ValidatedByName,
                    c.FechaValidacion AS ValidationDate, c.FechaCreacion AS CreatedAt,
                    f.UrlArchivo AS FileUrl,
                    o.Id, o.ConsignacionId AS ConsignationId, o.BancoDetectado AS DetectedBank, o.ReferenciaDetectada AS DetectedReference,
                    o.MontoDetectado AS DetectedAmount, o.FechaDetectada::timestamp AS DetectedDate,
                    o.Confianza AS Confidence, o.TextoCrudo AS RawText, o.FechaProcesamiento AS ProcessedAt
                FROM Flow_tblConsignaciones c
                INNER JOIN Flow_tblEmpresas comp ON c.EmpresaId = comp.Id
                INNER JOIN Flow_tblPuntosVenta s ON c.PuntoVentaId = s.Id
                INNER JOIN Flow_tblBancos b ON c.BancoId = b.Id
                INNER JOIN Flow_tblEstadosConsignacion est ON c.EstadoId = est.Id
                LEFT JOIN Flow_tblUsuarios u1 ON c.CreadoPor = u1.Id
                LEFT JOIN Flow_tblUsuarios u2 ON c.ValidadoPor = u2.Id
                LEFT JOIN Flow_tblArchivosConsignacion f ON c.Id = f.ConsignacionId
                LEFT JOIN Flow_tblResultadosOcr o ON c.Id = o.ConsignacionId
                WHERE (@IsAdmin = TRUE OR c.PuntoVentaId IN (
                    SELECT PuntoVentaId FROM Flow_tblUsuarioPuntosVenta WHERE UsuarioId = @UserId
                ))
                ORDER BY c.FechaCreacion DESC";

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
                    new { UserId = userId, IsAdmin = isAdmin },
                    splitOn: "Id"
                );

                return dict.Values;
            }
        }

        public async Task<GetConsignation> GetConsignationByIdAsync(long id)
        {
            var query = @"
                SELECT 
                    c.Id, c.EmpresaId AS CompanyId, comp.Nombre AS CompanyName, c.PuntoVentaId AS StoreId,
                    s.Nombre AS StoreName, c.BancoId AS BankId, b.Nombre AS BankName, c.EstadoId AS StatusId,
                    est.Nombre AS StatusName,
                    c.NumeroReferencia AS ReferenceNumber, c.MontoDeclarado AS DeclaredAmount, c.MontoDetectado AS DetectedAmount,
                    c.FechaConsignacion::timestamp AS ConsignationDate, c.HoraConsignacion::interval AS ConsignationTime,
                    c.Observaciones AS Notes, u1.NombreCompleto AS CreatedByName, u2.NombreCompleto AS ValidatedByName,
                    c.FechaValidacion AS ValidationDate, c.FechaCreacion AS CreatedAt,
                    f.UrlArchivo AS FileUrl,
                    o.Id, o.ConsignacionId AS ConsignationId, o.BancoDetectado AS DetectedBank, o.ReferenciaDetectada AS DetectedReference,
                    o.MontoDetectado AS DetectedAmount, o.FechaDetectada::timestamp AS DetectedDate,
                    o.Confianza AS Confidence, o.TextoCrudo AS RawText, o.FechaProcesamiento AS ProcessedAt
                FROM Flow_tblConsignaciones c
                INNER JOIN Flow_tblEmpresas comp ON c.EmpresaId = comp.Id
                INNER JOIN Flow_tblPuntosVenta s ON c.PuntoVentaId = s.Id
                INNER JOIN Flow_tblBancos b ON c.BancoId = b.Id
                INNER JOIN Flow_tblEstadosConsignacion est ON c.EstadoId = est.Id
                LEFT JOIN Flow_tblUsuarios u1 ON c.CreadoPor = u1.Id
                LEFT JOIN Flow_tblUsuarios u2 ON c.ValidadoPor = u2.Id
                LEFT JOIN Flow_tblArchivosConsignacion f ON c.Id = f.ConsignacionId
                LEFT JOIN Flow_tblResultadosOcr o ON c.Id = o.ConsignacionId
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
                    splitOn: "Id"
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
                            INSERT INTO Flow_tblConsignaciones 
                            (EmpresaId, PuntoVentaId, BancoId, EstadoId, NumeroReferencia, MontoDeclarado, MontoDetectado, 
                             FechaConsignacion, HoraConsignacion, Observaciones, CreadoPor, ValidadoPor, FechaValidacion, FechaCreacion)
                            VALUES 
                            (@CompanyId, @StoreId, @BankId, @StatusId, @ReferenceNumber, @DeclaredAmount, @DetectedAmount,
                             @ConsignationDate, @ConsignationTime, @Notes, @CreatedBy, @ValidatedBy, @ValidationDate, @CreatedAt)
                            RETURNING Id;";

                        var id = await connection.ExecuteScalarAsync<long>(insertConsignation, c, transaction);
                        c.Id = id;

                        var insertFile = @"
                            INSERT INTO Flow_tblArchivosConsignacion 
                            (ConsignacionId, NombreArchivo, UrlArchivo, TipoArchivo, TamanoArchivo, FechaSubida)
                            VALUES 
                            (@ConsignationId, @FileName, @FileUrl, @FileType, @FileSize, @UploadedAt);";
                        
                        f.ConsignationId = id;
                        await connection.ExecuteAsync(insertFile, f, transaction);

                        var insertOcr = @"
                            INSERT INTO Flow_tblResultadosOcr 
                            (ConsignacionId, BancoDetectado, ReferenciaDetectada, MontoDetectado, FechaDetectada, 
                             Confianza, TextoCrudo, FechaProcesamiento)
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
                UPDATE Flow_tblConsignaciones
                SET EstadoId = @StatusId, 
                    ValidadoPor = @ValidatorId, 
                    FechaValidacion = @ValidationDate, 
                    Observaciones = COALESCE(Observaciones, '') || CHR(10) || @Comments
                WHERE Id = @Id;";

            using (var connection = _context.CreateConnection())
            {
                var affected = await connection.ExecuteAsync(query, new { Id = id, StatusId = statusId, ValidatorId = validatorId, ValidationDate = DateTime.Now, Comments = comments });
                return affected > 0;
            }
        }
    }
}
