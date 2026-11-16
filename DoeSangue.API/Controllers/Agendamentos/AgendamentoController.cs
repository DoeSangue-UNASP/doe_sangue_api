using System.Security.Claims;
using DoeSangue.Application.Dtos.Agendamentos;
using DoeSangue.Application.UseCases.Agendamentos;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoeSangue.API.Controllers.Agendamentos
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AgendamentoController : ControllerBase
    {
        private readonly CriarAgendamentoUseCase _criarAgendamentoUseCase;
        private readonly BuscarAgendamentosDoadorUseCase _buscarAgendamentosDoadorUseCase;
        private readonly BuscarAgendaHemocentroUseCase _buscarAgendaHemocentroUseCase;
        private readonly CancelarAgendamentoUseCase _cancelarAgendamentoUseCase;
        private readonly ConcluirAgendamentoUseCase _concluirAgendamentoUseCase;

        public AgendamentoController(
            CriarAgendamentoUseCase criarAgendamentoUseCase,
            BuscarAgendamentosDoadorUseCase buscarAgendamentosDoadorUseCase,
            BuscarAgendaHemocentroUseCase buscarAgendaHemocentroUseCase,
            CancelarAgendamentoUseCase cancelarAgendamentoUseCase,
            ConcluirAgendamentoUseCase concluirAgendamentoUseCase)
        {
            _criarAgendamentoUseCase = criarAgendamentoUseCase;
            _buscarAgendamentosDoadorUseCase = buscarAgendamentosDoadorUseCase;
            _buscarAgendaHemocentroUseCase = buscarAgendaHemocentroUseCase;
            _cancelarAgendamentoUseCase = cancelarAgendamentoUseCase;
            _concluirAgendamentoUseCase = concluirAgendamentoUseCase;
        }

        [HttpPost]
        [Authorize(Roles = nameof(UsuarioRole.DOADOR))]
        public async Task<IActionResult> Criar([FromBody] CriarAgendamentoDto dto)
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            try
            {
                await _criarAgendamentoUseCase.Executar(usuarioId, dto);
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

        [HttpGet("me")]
        [Authorize(Roles = nameof(UsuarioRole.DOADOR))]
        public async Task<IActionResult> BuscarDoDoador()
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            var agendamentos = await _buscarAgendamentosDoadorUseCase.Executar(usuarioId);
            return Ok(agendamentos.Select(MapearAgendamento));
        }

        [HttpGet("hemocentro")]
        [Authorize(Roles = nameof(UsuarioRole.HEMOCENTRO))]
        public async Task<IActionResult> BuscarDoHemocentro([FromQuery] DateOnly? data, [FromQuery] string? documento)
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            var agendamentos = await _buscarAgendaHemocentroUseCase.Executar(usuarioId, data, documento);
            return Ok(agendamentos.Select(MapearAgendamento));
        }

        [HttpPatch("{agendamentoId:guid}/cancelar")]
        [Authorize(Roles = $"{nameof(UsuarioRole.DOADOR)},{nameof(UsuarioRole.HEMOCENTRO)}")]
        public async Task<IActionResult> Cancelar(Guid agendamentoId)
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            try
            {
                await _cancelarAgendamentoUseCase.Executar(agendamentoId, usuarioId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpPatch("{agendamentoId:guid}/concluir")]
        [Authorize(Roles = nameof(UsuarioRole.HEMOCENTRO))]
        public async Task<IActionResult> Concluir(Guid agendamentoId)
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            try
            {
                await _concluirAgendamentoUseCase.Executar(agendamentoId, usuarioId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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

        private static object MapearAgendamento(Agendamento agendamento)
        {
            return new
            {
                agendamento.Id,
                agendamento.AgendadoPara,
                agendamento.Status,
                Doador = new
                {
                    agendamento.Doador.Id,
                    agendamento.Doador.Nome,
                    agendamento.Doador.Documento,
                    agendamento.Doador.Telefone
                },
                Hemocentro = new
                {
                    agendamento.Hemocentro.Id,
                    agendamento.Hemocentro.NomeFantasia
                }
            };
        }
    }
}
