using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace AngularApp1.Server.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            
            // Auto-migration: Ensure status name is 'Lectura Ilegible' instead of 'Error IA'
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                // Rename status ID 4
                using (var cmd = new NpgsqlCommand("UPDATE flow_tblestadosconsignacion SET nombre = 'Lectura Ilegible' WHERE id = 4 AND nombre = 'Error IA'", connection))
                {
                    cmd.ExecuteNonQuery();
                }

                // Ensure 'Rechazada' status exists as ID 5
                using (var cmd = new NpgsqlCommand("INSERT INTO flow_tblestadosconsignacion (id, nombre) VALUES (5, 'Rechazada') ON CONFLICT (id) DO NOTHING", connection))
                {
                    cmd.ExecuteNonQuery();
                }

                // Create intermediate association table
                var createTableSql = @"
                    CREATE TABLE IF NOT EXISTS Flow_tblUsuarioPuntosVenta (
                        UsuarioId      BIGINT NOT NULL,
                        PuntoVentaId   BIGINT NOT NULL,
                        PRIMARY KEY (UsuarioId, PuntoVentaId),
                        CONSTRAINT fk_upv_usuario FOREIGN KEY(UsuarioId) REFERENCES Flow_tblUsuarios(Id) ON DELETE CASCADE,
                        CONSTRAINT fk_upv_punto FOREIGN KEY(PuntoVentaId) REFERENCES Flow_tblPuntosVenta(Id) ON DELETE CASCADE
                    );";
                using (var cmd = new NpgsqlCommand(createTableSql, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                // Seed intermediate table from existing User PuntoVentaId assignments
                var seedSql = @"
                    INSERT INTO Flow_tblUsuarioPuntosVenta (UsuarioId, PuntoVentaId)
                    SELECT Id, PuntoVentaId FROM Flow_tblUsuarios WHERE PuntoVentaId IS NOT NULL
                    ON CONFLICT DO NOTHING;";
                using (var cmd = new NpgsqlCommand(seedSql, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                // Seed default Company and Admin User if they do not exist
                long companyId = 0;
                using (var cmd = new NpgsqlCommand("SELECT Id FROM Flow_tblEmpresas LIMIT 1", connection))
                {
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        companyId = Convert.ToInt64(result);
                    }
                }

                if (companyId == 0)
                {
                    // Create default company
                    var insertCompanySql = @"
                        INSERT INTO Flow_tblEmpresas (Nombre, Nit, Email, Telefono, Direccion, Activo)
                        VALUES ('Letra Viva', '900.123.456-9', 'contacto@letraviva.com', '5550000', 'Dirección Principal Letra Viva', true)
                        RETURNING Id;";
                    using (var cmd = new NpgsqlCommand(insertCompanySql, connection))
                    {
                        companyId = Convert.ToInt64(cmd.ExecuteScalar());
                    }
                }

                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM Flow_tblUsuarios WHERE Email = 'murilloruiz91@gmail.com'", connection))
                {
                    var userExists = Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                    if (!userExists)
                    {
                        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
                        var hashedPassword = passwordHasher.HashPassword(new object(), "Admin123!");

                        var insertUserSql = @"
                            INSERT INTO Flow_tblUsuarios (EmpresaId, RolId, NombreCompleto, Email, PasswordHash, Activo)
                            VALUES (@EmpresaId, 1, 'Administrador Letra Viva', 'murilloruiz91@gmail.com', @PasswordHash, true);";
                        
                        using (var cmdInsert = new NpgsqlCommand(insertUserSql, connection))
                        {
                            cmdInsert.Parameters.AddWithValue("EmpresaId", companyId);
                            cmdInsert.Parameters.AddWithValue("PasswordHash", hashedPassword);
                            cmdInsert.ExecuteNonQuery();
                        }
                    }
                }

                // Seed Gerente: gerente@exito.com / Gerente123!
                long exitoCompanyId = 0;
                using (var cmd = new NpgsqlCommand("SELECT Id FROM Flow_tblEmpresas WHERE Nombre = 'Supermercados Éxito' LIMIT 1", connection))
                {
                    var result = cmd.ExecuteScalar();
                    if (result != null) exitoCompanyId = Convert.ToInt64(result);
                }
                if (exitoCompanyId == 0) exitoCompanyId = companyId; // fallback

                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM Flow_tblUsuarios WHERE Email = 'gerente@exito.com'", connection))
                {
                    var userExists = Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                    if (!userExists)
                    {
                        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
                        var hashedPassword = passwordHasher.HashPassword(new object(), "Gerente123!");

                        var insertSql = @"
                            INSERT INTO Flow_tblUsuarios (EmpresaId, RolId, NombreCompleto, Email, PasswordHash, Activo)
                            VALUES (@EmpresaId, 2, 'Gerente Supermercados Éxito', 'gerente@exito.com', @PasswordHash, true);";

                        using (var cmdInsert = new NpgsqlCommand(insertSql, connection))
                        {
                            cmdInsert.Parameters.AddWithValue("EmpresaId", exitoCompanyId);
                            cmdInsert.Parameters.AddWithValue("PasswordHash", hashedPassword);
                            cmdInsert.ExecuteNonQuery();
                        }
                    }
                }

                // Seed Cajero: cajero@exito.com / Cajero123!
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM Flow_tblUsuarios WHERE Email = 'cajero@exito.com'", connection))
                {
                    var userExists = Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                    if (!userExists)
                    {
                        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
                        var hashedPassword = passwordHasher.HashPassword(new object(), "Cajero123!");

                        var insertSql = @"
                            INSERT INTO Flow_tblUsuarios (EmpresaId, RolId, NombreCompleto, Email, PasswordHash, Activo)
                            VALUES (@EmpresaId, 3, 'Cajero Supermercados Éxito', 'cajero@exito.com', @PasswordHash, true);";

                        using (var cmdInsert = new NpgsqlCommand(insertSql, connection))
                        {
                            cmdInsert.Parameters.AddWithValue("EmpresaId", exitoCompanyId);
                            cmdInsert.Parameters.AddWithValue("PasswordHash", hashedPassword);
                            cmdInsert.ExecuteNonQuery();
                        }
                    }
                }

            }
            catch {}
        }

        public IDbConnection CreateConnection()
            => new NpgsqlConnection(_connectionString);
    }
}
