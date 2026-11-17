using System.Security.Claims;
using DoeSangue.Application.Dtos.Depoimentos;
using DoeSangue.Application.UseCases.Depoimentos;
using DoeSangue.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DoeSangue.API.Controllers.Depoimentos
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepoimentoController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly CriarDepoimentoUseCase _criarDepoimentoUseCase;
        private readonly BuscarDepoimentosUseCase _buscarDepoimentosUseCase;

        public DepoimentoController(IUsuarioRepository usuarioRepository, CriarDepoimentoUseCase criarDepoimentoUseCase, BuscarDepoimentosUseCase buscarDepoimentosUseCase)
        {
            _usuarioRepository = usuarioRepository;
            _criarDepoimentoUseCase = criarDepoimentoUseCase;
            _buscarDepoimentosUseCase = buscarDepoimentosUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarDepoimentoDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var autor = await _usuarioRepository.BuscarPorId(userId);

            if (autor is null)
            {
                return Unauthorized();
            }

            await _criarDepoimentoUseCase.Executar(autor, dto);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodos()
        {
            var depoimentos = await _buscarDepoimentosUseCase.Executar();
            return Ok(depoimentos.Select(d => new
            {
                d.Id,
                d.Titulo,
                d.Conteudo,
                d.CriadoEm,
                AutorEmail = d.Autor?.Email,
                AutorId = d.Autor?.Id
            }));
        }

        [HttpGet("publico")]
        [AllowAnonymous]
        [EnableRateLimiting("DepoimentosPublicos")]
        public async Task<IActionResult> BuscarDezPublicos()
        {
            var depoimentos = await _buscarDepoimentosUseCase.ExecutarPublicos();
            return Ok(depoimentos.Select(d => new
            {
                d.Id,
                d.Titulo,
                d.Conteudo,
                d.CriadoEm
            }));
        }
    }
}
