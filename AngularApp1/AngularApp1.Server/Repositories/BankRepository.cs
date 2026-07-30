using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using AngularApp1.Server.Data;
using AngularApp1.Server.Models;
using AngularApp1.Server.Models.SetModels;
using AngularApp1.Server.Models.ViewModels;

namespace AngularApp1.Server.Repositories
{
    public class BankRepository : IBankRepository
    {
        private readonly DapperContext _context;

        public BankRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetBank>> GetAllBanksAsync()
        {
            var query = @"
                SELECT 
                    Id, 
                    Nombre AS Name, 
                    Codigo AS Code 
                FROM Flow_tblBancos 
                ORDER BY Id ASC";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<GetBank>(query);
        }

        public async Task<GetBank?> GetBankByIdAsync(short id)
        {
            var query = @"
                SELECT 
                    Id, 
                    Nombre AS Name, 
                    Codigo AS Code 
                FROM Flow_tblBancos 
                WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<GetBank>(query, new { Id = id });
        }

        public async Task<Result> CreateBankAsync(PostBank model)
        {
            var query = @"
                INSERT INTO Flow_tblBancos (Nombre, Codigo) 
                VALUES (@Name, @Code)";

            var parameters = new DynamicParameters();
            parameters.Add("Name", model.Name);
            parameters.Add("Code", model.Code);

            try
            {
                using var connection = _context.CreateConnection();
                var rowsAffected = await connection.ExecuteAsync(query, parameters);

                if (rowsAffected > 0)
                {
                    return new Result { Success = true, Message = "Bank created successfully" };
                }
                return new Result { Success = false, Message = "Could not create the bank" };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<Result> UpdateBankAsync(short id, PostBank model)
        {
            var query = @"
                UPDATE Flow_tblBancos 
                SET Nombre = @Name, 
                    Codigo = @Code 
                WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("Name", model.Name);
            parameters.Add("Code", model.Code);

            try
            {
                using var connection = _context.CreateConnection();
                var rowsAffected = await connection.ExecuteAsync(query, parameters);

                if (rowsAffected > 0)
                {
                    return new Result { Success = true, Message = "Bank updated successfully" };
                }
                return new Result { Success = false, Message = "Bank not found or no changes made" };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<Result> DeleteBankAsync(short id)
        {
            var query = "DELETE FROM Flow_tblBancos WHERE Id = @Id";

            try
            {
                using var connection = _context.CreateConnection();
                var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });

                if (rowsAffected > 0)
                {
                    return new Result { Success = true, Message = "Bank deleted successfully" };
                }
                return new Result { Success = false, Message = "Bank not found" };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Error: {ex.Message}" };
            }
        }
    }
}
