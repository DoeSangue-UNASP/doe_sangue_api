using System.Security.Claims;
using DoeSangue.Application.UseCases.Doadores;
using DoeSangue.Domain.Enums;
using DoeSangue.Infrastructure.Data;
using DoeSangue.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoeSangue.API.Controllers.Doadores
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoadoresController : ControllerBase
    {
        private readonly BuscarTodosDoadoresUseCase _buscarTodosDoadoresUseCase;
        private readonly BuscarDoadorPorId _buscarDoadorPorId;
        private readonly DoeSangueContext _context;

        public DoadoresController(
            BuscarTodosDoadoresUseCase buscarTodosDoadoresUseCase,
            BuscarDoadorPorId buscarDoadorPorId,
            DoeSangueContext context)
        {
            _buscarTodosDoadoresUseCase = buscarTodosDoadoresUseCase;
            _buscarDoadorPorId = buscarDoadorPorId;
            _context = context;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UsuarioRole.HEMOCENTRO))]
        public async Task<IActionResult> BuscarTodos()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
            {
                return Unauthorized();
            }

            var doadores = await _buscarTodosDoadoresUseCase.Executar(usuarioId);
            var doadoresDto = doadores.Select(MapearDoador).ToList();

            return Ok(doadoresDto);
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UsuarioRole.HEMOCENTRO))]
        public async Task<IActionResult> BuscarPorId(Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
            {
                return Unauthorized();
            }

            var doador = await _buscarDoadorPorId.Executar(id, usuarioId);
            if (doador is null)
            {
                return NotFound();
            }

            return Ok(MapearDoador(doador));
        }

        [HttpGet("cpf/{cpf}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UsuarioRole.HEMOCENTRO))]
        public async Task<IActionResult> BuscarPorCpf(string cpf)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
            {
                return Unauthorized();
            }

            var hemocentroId = await _context.Hemocentros
                .Where(h => h.UsuarioId == usuarioId)
                .Select(h => h.Id)
                .FirstOrDefaultAsync();

            var doadores = await _context.Doadores
                .Where(d => d.Documento == cpf && (
                    d.HemocentroId == hemocentroId ||
                    _context.Agendamentos.Any(a => a.DoadorId == d.Id && a.Hemocentro.UsuarioId == usuarioId)
                ))
                .ToListAsync();

            if (doadores == null || !doadores.Any())
            {
                return Ok(new List<object>());
            }

            var resultado = doadores.Select(d => MapearDoador(new Domain.Entities.Doador
            {
                Id = d.Id,
                Nome = d.Nome,
                Documento = d.Documento,
                Telefone = d.Telefone,
                DataNascimento = d.DataNascimento,
                UsuarioId = d.UsuarioId,
                HemocentroId = d.HemocentroId
            })).ToList();

            return Ok(resultado);
        }

        private static object MapearDoador(Domain.Entities.Doador doador)
        {
            return new
            {
                doador.Id,
                doador.Nome,
                doador.Documento,
                doador.Telefone,
                doador.DataNascimento,
                ehLead = doador.EhLead,
                ehUsuario = doador.EhUsuario
            };
        }
    }
}
