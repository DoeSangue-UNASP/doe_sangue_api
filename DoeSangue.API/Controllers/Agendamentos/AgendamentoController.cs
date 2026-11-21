using System.Security.Claims;
using DoeSangue.Application.Dtos.Agendamentos;
using DoeSangue.Application.UseCases.Agendamentos;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Enums;
using DoeSangue.Domain.Interfaces;
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
        private readonly IDoadorRepository _doadorRepository;
        private readonly IHemocentroRepository _hemocentroRepository;
        private readonly IAgendamentoRepository _agendamentoRepository;

        public AgendamentoController(
            CriarAgendamentoUseCase criarAgendamentoUseCase,
            BuscarAgendamentosDoadorUseCase buscarAgendamentosDoadorUseCase,
            BuscarAgendaHemocentroUseCase buscarAgendaHemocentroUseCase,
            CancelarAgendamentoUseCase cancelarAgendamentoUseCase,
            ConcluirAgendamentoUseCase concluirAgendamentoUseCase,
            IDoadorRepository doadorRepository,
            IHemocentroRepository hemocentroRepository,
            IAgendamentoRepository agendamentoRepository)
        {
            _criarAgendamentoUseCase = criarAgendamentoUseCase;
            _buscarAgendamentosDoadorUseCase = buscarAgendamentosDoadorUseCase;
            _buscarAgendaHemocentroUseCase = buscarAgendaHemocentroUseCase;
            _cancelarAgendamentoUseCase = cancelarAgendamentoUseCase;
            _concluirAgendamentoUseCase = concluirAgendamentoUseCase;
            _doadorRepository = doadorRepository;
            _hemocentroRepository = hemocentroRepository;
            _agendamentoRepository = agendamentoRepository;
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

        [HttpPost("hemocentro")]
        [Authorize(Roles = nameof(UsuarioRole.HEMOCENTRO))]
        public async Task<IActionResult> CriarPeloHemocentro([FromBody] CriarAgendamentoDto dto)
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            try
            {
                var doador = await _doadorRepository.BuscarPorId(dto.DoadorId);
                if (doador is null)
                {
                    return NotFound("Doador inexistente.");
                }

                if (doador.EhLead)
                {
                    return BadRequest("Não é possível agendar uma doação com um lead. O doador deve se cadastrar na plataforma.");
                }

                var hemocentro = await _hemocentroRepository.BuscarPorId(dto.HemocentroId);
                if (hemocentro is null)
                {
                    return NotFound("Hemocentro inexistente.");
                }

                var horario = TimeOnly.FromTimeSpan(dto.AgendadoPara.TimeOfDay);
                var abertura = hemocentro.AbreEm;
                var fechamento = hemocentro.FechaEm;
                var horarioDisponivel = fechamento != abertura &&
                    (fechamento > abertura
                        ? horario >= abertura && horario < fechamento
                        : horario >= abertura || horario < fechamento);

                if (!horarioDisponivel)
                {
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, "O horário solicitado está fora do horário de funcionamento do hemocentro.");
                }

                if (await _agendamentoRepository.ExisteAgendamentoAtivo(hemocentro.Id, dto.AgendadoPara))
                {
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, "Já existe um agendamento ativo para esse horário.");
                }

                var agendamento = Agendamento.Criar(dto.AgendadoPara, hemocentro, doador);
                await _agendamentoRepository.Criar(agendamento);

                return StatusCode(StatusCodes.Status201Created);
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
        public async Task<IActionResult> BuscarDoHemocentro(
            [FromQuery] DateOnly? data,
            [FromHeader(Name = "documento")] string? documento)
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
