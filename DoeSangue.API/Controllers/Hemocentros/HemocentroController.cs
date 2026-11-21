using System.Security.Claims;
using DoeSangue.Application.Dtos.Doador;
using DoeSangue.Application.Dtos.Hemocentros;
using DoeSangue.Application.UseCases.Doadores;
using DoeSangue.Application.UseCases.Hemocentros;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Enums;
using DoeSangue.Infrastructure.Data;
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
        private readonly CadastrarLeadUseCase _cadastrarLeadUseCase;
        private readonly DoeSangueContext _context;

        public HemocentroController(
            CriarHemocentroUseCase criarHemocentroUseCase,
            BuscarHemocentroPorUsuarioIdUseCase buscarHemocentroPorUsuarioIdUseCase,
            CadastrarLeadUseCase cadastrarLeadUseCase,
            DoeSangueContext context)
        {
            _criarHemocentroUseCase = criarHemocentroUseCase;
            _buscarHemocentroPorUsuarioIdUseCase = buscarHemocentroPorUsuarioIdUseCase;
            _cadastrarLeadUseCase = cadastrarLeadUseCase;
            _context = context;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Criar([FromBody] CriarHemocentroDto hemocentroDto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _criarHemocentroUseCase.Executar(hemocentroDto);
                await transaction.CommitAsync();

                return Created();
            }
            catch (ArgumentException exception)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, exception.Message);
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

        [HttpPost("leads")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UsuarioRole.HEMOCENTRO))]
        public async Task<IActionResult> CadastrarLead([FromBody] CadastrarLeadDto leadDto)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
            {
                return Unauthorized();
            }

            try
            {
                await _cadastrarLeadUseCase.Executar(leadDto, usuarioId);
                return Created();
            }
            catch (InvalidOperationException exception)
            {
                return NotFound(exception.Message);
            }
            catch (ArgumentException exception)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, exception.Message);
            }
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

