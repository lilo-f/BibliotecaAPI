using BibliotecaAPI.Models;
using BibliotecaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmprestimoController : ControllerBase
    {
        private readonly EmprestimoRepository _emprestimoRepository;

        public EmprestimoController(EmprestimoRepository emprestimoRepository)
        {
            _emprestimoRepository = emprestimoRepository;
        }

        [HttpGet("listar-emprestimos")]
        public async Task<IActionResult> ListarEmprestimos()
        {
            var emprestimos = await _emprestimoRepository.ListarEmprestimosDB();
            return Ok(emprestimos);
        }

        [HttpPost("registrar-emprestimo")]
        public async Task<IActionResult> RegistrarEmprestimo([FromBody] Emprestimo emprestimo)
        {
            try
            {
                emprestimo.DataEmprestimo = DateTime.Now;
                var emprestimoId = await _emprestimoRepository.RegistrarEmprestimoDB(emprestimo);

                return Ok(new { mensagem = "Empréstimo registrado!!", emprestimoId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPut("registrar-devolucao/{id}")]
        public async Task<IActionResult> RegistrarDevolucao(int id)
        {
            var dataDevolucao = DateTime.Now;
            await _emprestimoRepository.RegistrarDevolucaoDB(id, dataDevolucao);

            return Ok(new { mensagem = "Devolução registrada!", dataDevolucao });
        }
    }
}
