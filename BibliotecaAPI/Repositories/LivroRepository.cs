using BibliotecaAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace BibliotecaAPI.Repositories
{
    public class LivroRepository
    {
        private readonly string _connectionString;

        public LivroRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private readonly EmprestimoRepository _emprestimoRepository;


        public LivroRepository(string connectionString, EmprestimoRepository emprestimoRepository)
        {
            _connectionString = connectionString;
            _emprestimoRepository = emprestimoRepository;
        }

        private IDbConnection Connection => new MySqlConnection(_connectionString);

        public async Task<IEnumerable<Livro>> ListarLivrosDB()
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Livros";
                return await conn.QueryAsync<Livro>(sql);
            }
        }

        public async Task<Livro> ObterLivroPorIdDB(int id)
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Livros WHERE Id = @Id";
                return await conn.QueryFirstOrDefaultAsync<Livro>(sql, new { Id = id });
            }
        }

        public async Task<int> RegistrarLivroDB(Livro livro)
        {
            using (var conn = Connection)
            {
                var sql = "INSERT INTO Livros (Titulo, Autor, AnoPublicacao, Genero, Disponivel) " +
                          "VALUES (@Titulo, @Autor, @AnoPublicacao, @Genero, @Disponivel);" +
                          "SELECT LAST_INSERT_ID();";
                return await conn.ExecuteScalarAsync<int>(sql, livro);
            }
        }

        public async Task<int> AtualizarLivroDB(Livro livro)
        {
            using (var conn = Connection)
            {
                var sql = "UPDATE Livros SET Titulo = @Titulo, Autor = @Autor, AnoPublicacao = @AnoPublicacao, " +
                          "Genero = @Genero, Disponivel = @Disponivel WHERE Id = @Id";
                return await conn.ExecuteAsync(sql, livro);
            }
        }

        public async Task<int> ExcluirLivroDB(int id)
        {
            // Verificar se o livro está emprestado
            bool emprestado = await _emprestimoRepository.LivroEmprestado(id);
            if (emprestado)
            {
                throw new InvalidOperationException("O livro está emprestado e não pode ser excluído.");
            }

            using (var conn = Connection)
            {
                var sqlExcluirLivro = "DELETE FROM Livros WHERE Id = @Id";
                return await conn.ExecuteAsync(sqlExcluirLivro, new { Id = id });
            }
        }

        public async Task<bool> VerificarDisponibilidadeLivro(int livroId)
        {
            using (var conn = Connection)
            {
                var sql = "SELECT COUNT(*) FROM Livros WHERE Id = @LivroId AND Disponivel = TRUE";
                var count = await conn.ExecuteScalarAsync<int>(sql, new { LivroId = livroId });

                return count > 0;
            }
        }

        public async Task<IEnumerable<Livro>> ConsultarLivros(string? genero = null, string? autor = null, int? anoPublicacao = null)
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Livros WHERE 1=1";

                if (!string.IsNullOrEmpty(genero))
                {
                    sql += " AND Genero = @Genero";
                }
                if (!string.IsNullOrEmpty(autor))
                {
                    sql += " AND Autor = @Autor";
                }
                if (anoPublicacao.HasValue)
                {
                    sql += " AND AnoPublicacao = @AnoPublicacao";
                }

                return await conn.QueryAsync<Livro>(sql, new { Genero = genero, Autor = autor, AnoPublicacao = anoPublicacao });
            }
        }

        
    }
}
