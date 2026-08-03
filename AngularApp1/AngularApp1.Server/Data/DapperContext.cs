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
            }
            catch {}
        }

        public IDbConnection CreateConnection()
            => new NpgsqlConnection(_connectionString);
    }
}
