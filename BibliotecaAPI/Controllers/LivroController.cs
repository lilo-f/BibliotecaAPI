using BibliotecaAPI.Models;
using BibliotecaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LivroController : ControllerBase
    {
        private readonly LivroRepository _livroRepository;

        public LivroController(LivroRepository livroRepository)
        {
            _livroRepository = livroRepository;
        }

        [HttpGet("listar-livros")]
        public async Task<IActionResult> ListarLivros()
        {
            var livros = await _livroRepository.ListarLivrosDB();

            return Ok(livros);
        }

        [HttpGet("consultar-livros")]
        public async Task<IActionResult> ConsultarLivros([FromQuery] string? genero, [FromQuery] string? autor, [FromQuery] int? anoPublicacao)
        {
            var livros = await _livroRepository.ConsultarLivros(genero, autor, anoPublicacao);
            return Ok(livros);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterLivro(int id)
        {
            var livro = await _livroRepository.ObterLivroPorIdDB(id);

            if (livro == null)
                return NotFound("Livro não encontrado");
            return Ok(livro);
        }

        [HttpPost("registrar-livro")]
        public async Task<IActionResult> RegistrarLivro([FromBody] Livro livro)
        {
            var livroId = await _livroRepository.RegistrarLivroDB(livro);

            return Ok(new { mensagem = "Livro registrado com sucesso", livroId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarLivro(int id, [FromBody] Livro livro)
        {
            livro.Id = id;
            await _livroRepository.AtualizarLivroDB(livro);

            return Ok(new { mensagem = "Livro atualizado" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirLivro(int id)
        {
            try
            {
                await _livroRepository.ExcluirLivroDB(id);

                return Ok(new { mensagem = "Livro excluído com sucesso" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}
