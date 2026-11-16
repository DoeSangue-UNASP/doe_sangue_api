using System.Security.Claims;
using DoeSangue.Application.Dtos.Hemocentros;
using DoeSangue.Application.UseCases.Hemocentros;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoeSangue.API.Controllers.Hemocentros
{
    [Route("api/[controller]")]
    [ApiController]
    public class HemocentroController : ControllerBase
    {
        private readonly CriarHemocentroUseCase _criarHemocentroUseCase;
        private readonly BuscarHemocentroPorUsuarioIdUseCase _buscarHemocentroPorUsuarioIdUseCase;

        public HemocentroController(
            CriarHemocentroUseCase criarHemocentroUseCase,
            BuscarHemocentroPorUsuarioIdUseCase buscarHemocentroPorUsuarioIdUseCase)
        {
            _criarHemocentroUseCase = criarHemocentroUseCase;
            _buscarHemocentroPorUsuarioIdUseCase = buscarHemocentroPorUsuarioIdUseCase;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Criar([FromBody] CriarHemocentroDto hemocentroDto)
        {
            try
            {
                await _criarHemocentroUseCase.Executar(hemocentroDto);

                return Created();
            }
            catch (ArgumentException exception)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, exception.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UsuarioRole.HEMOCENTRO))]
        public async Task<IActionResult> BuscarMeuPerfil()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
            {
                return Unauthorized();
            }

            var hemocentro = await _buscarHemocentroPorUsuarioIdUseCase.Executar(usuarioId);
            return hemocentro is null ? NotFound() : Ok(MapearHemocentro(hemocentro));
        }

        private static object MapearHemocentro(Hemocentro hemocentro)
        {
            return new
            {
                hemocentro.Id,
                hemocentro.Cnpj,
                hemocentro.Site,
                hemocentro.NomeFantasia,
                hemocentro.AbreEm,
                hemocentro.FechaEm
            };
        }
    }
}
