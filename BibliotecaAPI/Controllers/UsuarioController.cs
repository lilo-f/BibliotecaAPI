using BibliotecaAPI.Models;
using BibliotecaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioController(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("listar-usuarios")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarios = await _usuarioRepository.ListarUsuariosDB();
            return Ok(usuarios);
        }

        [HttpGet("buscar-usuarios")]
        public async Task<IActionResult> BuscarUsuarios([FromQuery] string? nome, [FromQuery] string? email)
        {
            var usuarios = await _usuarioRepository.BuscarUsuarios(nome, email);
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterUsuario(int id)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorIdDB(id);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            return Ok(usuario);
        }

        [HttpPost("registrar-usuario")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] Usuario usuario)
        {
            var usuarioId = await _usuarioRepository.RegistrarUsuarioDB(usuario);

            return Ok(new { mensagem = "Usuário cadastrado com sucesso", usuarioId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarUsuario(int id, [FromBody] Usuario usuario)
        {
            usuario.Id = id;
            await _usuarioRepository.AtualizarUsuarioDB(usuario);

            return Ok(new { mensagem = "Usuário atualizado com sucesso" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirUsuario(int id)
        {
            await _usuarioRepository.ExcluirUsuarioDB(id);

            return Ok(new { mensagem = "Usuário excluído com sucesso" });
        }

    }
}
