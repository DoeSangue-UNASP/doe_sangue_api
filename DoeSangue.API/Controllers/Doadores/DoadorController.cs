using System.Security.Claims;
using DoeSangue.Application.Dtos.Doador;
using DoeSangue.Application.UseCases.Doadores;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Enums;
using DoeSangue.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoeSangue.API.Controllers.Doadores
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DoadorController : ControllerBase
    {
        private readonly CriarDoadorUseCase _criarDoadorUseCase;
        private readonly BuscarDoadorPorId _buscarDoadorPorId;
        private readonly BuscarDoadorPorUsuarioIdUseCase _buscarDoadorPorUsuarioIdUseCase;
        private readonly BuscarTodosDoadoresUseCase _buscarTodosDoadoresUseCase;
        private readonly ExcluirDoadorPorUsuarioId _excluirDoadorPorUsuarioId;
        private readonly AtualizarDoadorPorId _atualizarDoadorPorId;
        private readonly DoeSangueContext _context;

        public DoadorController(
            CriarDoadorUseCase criarDoadorUseCase,
            BuscarDoadorPorId buscarDoadorPorUsuarioId,
            BuscarDoadorPorUsuarioIdUseCase buscarDoadorPorUsuarioIdUseCase,
            BuscarTodosDoadoresUseCase buscarTodosDoadoresUseCase,
            ExcluirDoadorPorUsuarioId excluirDoadorPorUsuarioId,
            AtualizarDoadorPorId atualizarDoadorPorId,
            DoeSangueContext context)
        {
            _criarDoadorUseCase = criarDoadorUseCase;
            _buscarDoadorPorId = buscarDoadorPorUsuarioId;
            _buscarDoadorPorUsuarioIdUseCase = buscarDoadorPorUsuarioIdUseCase;
            _buscarTodosDoadoresUseCase = buscarTodosDoadoresUseCase;
            _excluirDoadorPorUsuarioId = excluirDoadorPorUsuarioId;
            _atualizarDoadorPorId = atualizarDoadorPorId;
            _context = context;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Criar([FromBody] CriarDoadorDto doadorDto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _criarDoadorUseCase.Executar(doadorDto);
                await transaction.CommitAsync();
                return Created();
            }
            catch (ArgumentException exception)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, exception.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = nameof(UsuarioRole.HEMOCENTRO))]
        public async Task<IActionResult> BuscarTodos()
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            var doadores = await _buscarTodosDoadoresUseCase.Executar(usuarioId);
            return Ok(doadores.Select(MapearDoador));
        }

        [HttpGet("me")]
        [Authorize(Roles = nameof(UsuarioRole.DOADOR))]
        public async Task<IActionResult> BuscarMeuPerfil()
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            var doador = await _buscarDoadorPorUsuarioIdUseCase.Executar(usuarioId);
            return doador is null ? NotFound() : Ok(MapearMeuPerfil(doador));
        }

        [HttpGet("{doadorId:guid}")]
        [Authorize(Roles = nameof(UsuarioRole.HEMOCENTRO))]
        public async Task<IActionResult> BuscarPorId(Guid doadorId)
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            var doador = await _buscarDoadorPorId.Executar(doadorId, usuarioId);
            return doador is null ? NotFound() : Ok(MapearDoador(doador));
        }

        [HttpDelete("me")]
        [Authorize(Roles = nameof(UsuarioRole.DOADOR))]
        public async Task<IActionResult> Remover()
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            try
            {
                await _excluirDoadorPorUsuarioId.Executar(usuarioId);

                return NoContent();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPut("me")]
        [Authorize(Roles = nameof(UsuarioRole.DOADOR))]
        public async Task<IActionResult> Atualizar([FromBody] AtualizarDoadorDto doadorDto)
        {
            if (!ObterUsuarioId(out var usuarioId))
            {
                return Unauthorized();
            }

            try
            {
                var doador = await _buscarDoadorPorUsuarioIdUseCase.Executar(usuarioId);
                if (doador is null)
                {
                    return NotFound();
                }

                await _atualizarDoadorPorId.Executar(doador.Id, doadorDto);

                return NoContent();
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

        private bool ObterUsuarioId(out Guid usuarioId)
        {
            return Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out usuarioId);
        }

        private static object MapearMeuPerfil(Doador doador)
        {
            var endereco = doador.Usuario?.Endereco;

            return new
            {
                doador.Id,
                doador.Nome,
                doador.Documento,
                doador.Telefone,
                doador.DataNascimento,
                Email = doador.Usuario?.Email,
                Endereco = endereco is null ? null : new
                {
                    endereco.Cep,
                    endereco.Numero,
                    endereco.Rua,
                    endereco.Bairro,
                    endereco.Cidade,
                    endereco.Estado
                }
            };
        }

        private static object MapearDoador(Doador doador)
        {
            return new
            {
                doador.Id,
                doador.Nome,
                doador.Documento,
                doador.Telefone,
                doador.DataNascimento
            };
        }
    }
}
