using BibliotecaAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace BibliotecaAPI.Repositories
{
    public class UsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection Connection => new MySqlConnection(_connectionString);

        public async Task<IEnumerable<Usuario>> ListarUsuariosDB()
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Usuarios";
                return await conn.QueryAsync<Usuario>(sql);
            }
        }

        public async Task<Usuario> ObterUsuarioPorIdDB(int id)
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Usuarios WHERE Id = @Id";
                return await conn.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
            }
        }

        public async Task<int> RegistrarUsuarioDB(Usuario usuario)
        {
            using (var conn = Connection)
            {
                var sql = "INSERT INTO Usuarios (Nome, Email) VALUES (@Nome, @Email);" +
                          "SELECT LAST_INSERT_ID();";
                return await conn.ExecuteScalarAsync<int>(sql, usuario);
            }
        }

        public async Task<int> AtualizarUsuarioDB(Usuario usuario)
        {
            using (var conn = Connection)
            {
                var sql = "UPDATE Usuarios SET Nome = @Nome, Email = @Email WHERE Id = @Id";
                return await conn.ExecuteAsync(sql, usuario);
            }
        }

        public async Task<int> ExcluirUsuarioDB(int id)
        {
            using (var conn = Connection)
            {
                var sql = "DELETE FROM Usuarios WHERE Id = @Id";
                return await conn.ExecuteAsync(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<Usuario>> BuscarUsuarios(string? nome = null, string? email = null)
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Usuarios WHERE 1=1";

                if (!string.IsNullOrEmpty(nome))
                {
                    sql += " AND Nome LIKE @Nome";
                }
                if (!string.IsNullOrEmpty(email))
                {
                    sql += " AND Email LIKE @Email";
                }

                return await conn.QueryAsync<Usuario>(sql, new { Nome = $"%{nome}%", Email = $"%{email}%" });
            }
        }
    }
}
