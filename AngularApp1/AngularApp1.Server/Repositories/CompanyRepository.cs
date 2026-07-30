using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using AngularApp1.Server.Data;
using AngularApp1.Server.Models.Entities;
using AngularApp1.Server.Models.SetModels;
using AngularApp1.Server.Models.ViewModels;
using AngularApp1.Server.Models;

namespace AngularApp1.Server.Repositories
{
    public class CompanyRepository
    {
        private readonly DapperContext _context;

        public CompanyRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetCompany>> GetAllCompaniesAsync()
        {
            var query = @"
                SELECT 
                    Id, 
                    Nombre AS Name, 
                    Nit, 
                    Email, 
                    Telefono AS Phone, 
                    Direccion AS Address, 
                    UrlLogo AS LogoUrl, 
                    Activo AS IsActive, 
                    FechaCreacion AS CreatedAt 
                FROM Flow_tblEmpresas 
                ORDER BY Id DESC";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<GetCompany>(query);
        }

        public async Task<GetCompany?> GetCompanyByIdAsync(long id)
        {
            var query = @"
                SELECT 
                    Id, 
                    Nombre AS Name, 
                    Nit, 
                    Email, 
                    Telefono AS Phone, 
                    Direccion AS Address, 
                    UrlLogo AS LogoUrl, 
                    Activo AS IsActive, 
                    FechaCreacion AS CreatedAt 
                FROM Flow_tblEmpresas 
                WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<GetCompany>(query, new { Id = id });
        }

        public async Task<Result> CreateCompanyAsync(PostCompany model)
        {
            var query = @"
                INSERT INTO Flow_tblEmpresas (Nombre, Nit, Email, Telefono, Direccion, UrlLogo) 
                VALUES (@Name, @Nit, @Email, @Phone, @Address, @LogoUrl)";

            var parameters = new DynamicParameters();
            parameters.Add("Name", model.Name);
            parameters.Add("Nit", model.Nit);
            parameters.Add("Email", model.Email);
            parameters.Add("Phone", model.Phone);
            parameters.Add("Address", model.Address);
            parameters.Add("LogoUrl", model.LogoUrl);

            try
            {
                using var connection = _context.CreateConnection();
                var rowsAffected = await connection.ExecuteAsync(query, parameters);

                if (rowsAffected > 0)
                {
                    return new Result { Success = true, Message = "Company created successfully" };
                }
                return new Result { Success = false, Message = "Could not create the company" };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<Result> UpdateCompanyAsync(long id, PostCompany model)
        {
            var query = @"
                UPDATE Flow_tblEmpresas
                SET Nombre = @Name,
                    Nit = @Nit,
                    Email = @Email,
                    Telefono = @Phone,
                    Direccion = @Address,
                    UrlLogo = @LogoUrl,
                    Activo = @IsActive
                WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("Name", model.Name);
            parameters.Add("Nit", model.Nit);
            parameters.Add("Email", model.Email);
            parameters.Add("Phone", model.Phone);
            parameters.Add("Address", model.Address);
            parameters.Add("LogoUrl", model.LogoUrl);
            parameters.Add("IsActive", model.IsActive);

            try
            {
                using var connection = _context.CreateConnection();
                var rowsAffected = await connection.ExecuteAsync(query, parameters);

                if (rowsAffected > 0)
                {
                    return new Result { Success = true, Message = "Company updated successfully" };
                }
                return new Result { Success = false, Message = "Company not found or no changes made" };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<Result> DeleteCompanyAsync(long id)
        {
            var query = "DELETE FROM Flow_tblEmpresas WHERE Id = @Id";

            try
            {
                using var connection = _context.CreateConnection();
                var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });

                if (rowsAffected > 0)
                {
                    return new Result { Success = true, Message = "Company deleted successfully" };
                }
                return new Result { Success = false, Message = "Company not found" };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Error: {ex.Message}" };
            }
        }
    }
}
