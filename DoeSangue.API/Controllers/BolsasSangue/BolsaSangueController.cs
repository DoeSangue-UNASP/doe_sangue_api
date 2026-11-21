using System.Security.Claims;
using DoeSangue.Application.Dtos.BolsasSangue;
using DoeSangue.Application.UseCases.BolsasSangue;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoeSangue.API.Controllers.BolsasSangue
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BolsaSangueController : ControllerBase
    {
        private readonly CriarBolsaSangueUseCase _criarBolsaSangueUseCase;
        private readonly BuscarBolsasSangueUseCase _buscarBolsasSangueUseCase;
        private readonly BuscarBolsaSangueUseCase _buscarBolsaSangueUseCase;
        private readonly AtualizarStatusBolsaUseCase _atualizarStatusBolsaUseCase;

        public BolsaSangueController(
            CriarBolsaSangueUseCase criarBolsaSangueUseCase,
            BuscarBolsasSangueUseCase buscarBolsasSangueUseCase,
            BuscarBolsaSangueUseCase buscarBolsaSangueUseCase,
            AtualizarStatusBolsaUseCase atualizarStatusBolsaUseCase)
        {
            _criarBolsaSangueUseCase = criarBolsaSangueUseCase;
            _buscarBolsasSangueUseCase = buscarBolsasSangueUseCase;
            _buscarBolsaSangueUseCase = buscarBolsaSangueUseCase;
            _atualizarStatusBolsaUseCase = atualizarStatusBolsaUseCase;
        }

        [HttpPost]
        [Authorize(Roles = nameof(UsuarioRole.HEMOCENTRO))]
        public async Task<IActionResult> Criar([FromBody] CriarBolsaSangueDto dto)
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            try
            {
                await _criarBolsaSangueUseCase.Executar(usuarioId, dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("hemocentro")]
        [Authorize(Roles = nameof(UsuarioRole.HEMOCENTRO))]
        public async Task<IActionResult> BuscarDoHemocentro()
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            var bolsas = await _buscarBolsasSangueUseCase.Executar(usuarioId);
            return Ok(new
            {
                Bolsas = bolsas.Select(MapearBolsa),
                Estoque = bolsas
                    .GroupBy(x => x.Status)
                    .Select(grupo => new { Status = grupo.Key, Quantidade = grupo.Count() })
            });
        }

        [HttpGet("{bolsaId:guid}")]
        [Authorize(Roles = nameof(UsuarioRole.HEMOCENTRO))]
        public async Task<IActionResult> BuscarPorId(Guid bolsaId)
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            var bolsa = await _buscarBolsaSangueUseCase.Executar(bolsaId, usuarioId);
            return bolsa is null ? NotFound() : Ok(MapearBolsa(bolsa));
        }

        [HttpPatch("{bolsaId:guid}/status")]
        [Authorize(Roles = nameof(UsuarioRole.HEMOCENTRO))]
        public async Task<IActionResult> AtualizarStatus(Guid bolsaId, [FromBody] AtualizarStatusBolsaDto dto)
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            try
            {
                await _atualizarStatusBolsaUseCase.Executar(bolsaId, usuarioId, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        private bool ObterUsuarioId(out Guid usuarioId)
        {
            return Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out usuarioId);
        }

        private static object MapearBolsa(BolsaSangue bolsa)
        {
            return new
            {
                bolsa.Id,
                bolsa.Codigo,
                bolsa.TesteSorologico,
                bolsa.TipoComponente,
                bolsa.FatorRh,
                bolsa.TipoSanguineo,
                bolsa.Volume,
                bolsa.Status,
                bolsa.CriadoEm,
                bolsa.DataColeta,
                bolsa.DataValidade,
                bolsa.LocalColeta,
                bolsa.Destino,
                bolsa.ResponsavelColeta,
                AgendamentoId = bolsa.Agendamento.Id,
                AgendadoPara = bolsa.Agendamento.AgendadoPara,
                Hemocentro = bolsa.Agendamento.Hemocentro.NomeFantasia,
                DoadorId = bolsa.Agendamento.Doador.Id,
                Doador = bolsa.Agendamento.Doador.Nome
            };
        }
    }
}
