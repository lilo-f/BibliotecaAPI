using BibliotecaAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using System.Transactions;

namespace BibliotecaAPI.Repositories
{
    public class EmprestimoRepository
    {
        private readonly string _connectionString;

        public EmprestimoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private readonly LivroRepository _livroRepository;

        public EmprestimoRepository(string connectionString, LivroRepository livroRepository)
        {
            _connectionString = connectionString;
            _livroRepository = livroRepository;
        }

        private IDbConnection Connection => new MySqlConnection(_connectionString);

        public async Task<IEnumerable<Emprestimo>> ListarEmprestimosDB()
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Emprestimos";
                return await conn.QueryAsync<Emprestimo>(sql);
            }
        }

        public async Task<int> RegistrarEmprestimoDB(Emprestimo emprestimo)
        {
            using (var conn = Connection)
            {
                using (var transaction = conn.BeginTransaction())
                {
                    bool disponivel = await _livroRepository.VerificarDisponibilidadeLivro(emprestimo.LivroId);

                    if (!disponivel)
                    {
                        throw new InvalidOperationException("Livro não está disponível para empréstimo.");
                    }

                    emprestimo.DataEmprestimo = DateTime.Now;
                    emprestimo.DataDevolucao = emprestimo.DataEmprestimo.AddDays(14);

                    var sqlRegistrarEmprestimo = "INSERT INTO Emprestimos (LivroId, UsuarioId, DataEmprestimo) " +
                                                 "VALUES (@LivroId, @UsuarioId, @DataEmprestimo);" +
                                                 "SELECT LAST_INSERT_ID();";

                    var emprestimoId = await conn.ExecuteScalarAsync<int>(sqlRegistrarEmprestimo, emprestimo, transaction);

                    var sqlAtualizarLivro = "UPDATE Livros SET Disponivel = FALSE WHERE Id = @LivroId";
                    await conn.ExecuteAsync(sqlAtualizarLivro, new { emprestimo.LivroId }, transaction);

                    transaction.Commit();
                    return emprestimoId;
                }
            }
        }

        public async Task<int> RegistrarDevolucaoDB(int emprestimoId, DateTime dataDevolucao)
        {
            using (var conn = Connection)
            {
                var sql = "UPDATE Emprestimos SET DataDevolucao = @DataDevolucao WHERE Id = @EmprestimoId;" +
                          "UPDATE Livros SET Disponivel = true WHERE Id = (SELECT LivroId FROM Emprestimos WHERE Id = @EmprestimoId)";
                return await conn.ExecuteAsync(sql, new { DataDevolucao = dataDevolucao, EmprestimoId = emprestimoId });
            }
        }

        public async Task<Emprestimo> ObterEmprestimoPorIdDB(int id)
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Emprestimos WHERE Id = @Id";
                return await conn.QueryFirstOrDefaultAsync<Emprestimo>(sql, new { Id = id });
            }
        }

        public async Task<bool> LivroEmprestado(int livroId)
        {
            using (var conn = Connection)
            {
                var sql = "SELECT COUNT(*) FROM Emprestimos WHERE LivroId = @LivroId AND DataDevolucao IS NULL";
                var count = await conn.ExecuteScalarAsync<int>(sql, new { LivroId = livroId });

                return count > 0;
            }
        }
    }
}
