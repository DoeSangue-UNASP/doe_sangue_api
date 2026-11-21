using System.Data;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Enums;
using DoeSangue.Domain.Interfaces;
using DoeSangue.Infrastructure.Data;
using DoeSangue.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace DoeSangue.Infrastructure.Repository
{
    public class AgendamentoRepository : IAgendamentoRepository
    {
        private readonly DoeSangueContext _context;

        public AgendamentoRepository(DoeSangueContext context)
        {
            _context = context;
        }

        public async Task Criar(Agendamento agendamento)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var horarioOcupado = await _context.Agendamentos.AnyAsync(x =>
                x.HemocentroId == agendamento.Hemocentro.Id &&
                x.AgendadoPara == agendamento.AgendadoPara &&
                x.Status == StatusAgendamento.AGENDADO);

            if (horarioOcupado)
            {
                throw new ArgumentException("Já existe um agendamento ativo para esse horário.");
            }

            var agendamentoModel = new AgendamentoModel
            {
                Id = agendamento.Id,
                AtualizadoEm = agendamento.AtualizadoEm,
                CriadoEm = agendamento.CriadoEm,
                AgendadoPara = agendamento.AgendadoPara,
                Status = agendamento.Status,
                HemocentroId = agendamento.Hemocentro.Id,
                DoadorId = agendamento.Doador.Id
            };

            await _context.Agendamentos.AddAsync(agendamentoModel);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task<List<Agendamento>> BuscarPorUsuarioDoador(Guid usuarioId)
        {
            var models = await _context.Agendamentos
                .Include(x => x.Doador)
                .Include(x => x.Hemocentro)
                .Where(x => x.Doador.UsuarioId == usuarioId)
                .AsNoTracking()
                .ToListAsync();

            return models.Select(MapToDomain).ToList();
        }

        public async Task<List<Agendamento>> BuscarPorUsuarioHemocentro(Guid usuarioId, DateOnly? data, string? documento)
        {
            var query = _context.Agendamentos
                .Include(x => x.Doador)
                .Include(x => x.Hemocentro)
                .Where(x => x.Hemocentro.UsuarioId == usuarioId);

            if (data.HasValue)
            {
                var inicio = data.Value.ToDateTime(TimeOnly.MinValue);
                var fim = inicio.AddDays(1);
                query = query.Where(x => x.AgendadoPara >= inicio && x.AgendadoPara < fim);
            }

            if (!string.IsNullOrWhiteSpace(documento))
            {
                query = query.Where(x => x.Doador.Documento == documento);
            }

            var models = await query
                .AsNoTracking()
                .OrderBy(x => x.AgendadoPara)
                .ToListAsync();

            return models.Select(MapToDomain).ToList();
        }

        public async Task<Agendamento?> BuscarPorId(Guid agendamentoId)
        {
            var model = await _context.Agendamentos
                .Include(x => x.Doador)
                .Include(x => x.Hemocentro)
                .FirstOrDefaultAsync(x => x.Id == agendamentoId);

            if (model is null)
            {
                return null;
            }

            return MapToDomain(model);
        }

        public async Task<bool> ExisteAgendamentoAtivo(Guid hemocentroId, DateTime agendadoPara)
        {
            return await _context.Agendamentos.AnyAsync(x =>
                x.HemocentroId == hemocentroId &&
                x.AgendadoPara == agendadoPara &&
                x.Status == StatusAgendamento.AGENDADO);
        }

        public async Task Atualizar(Agendamento agendamento)
        {
            var model = await _context.Agendamentos
                .FirstOrDefaultAsync(x => x.Id == agendamento.Id);

            if (model is null)
            {
                throw new ArgumentException("Agendamento inexistente.");
            }

            model.Status = agendamento.Status;
            model.AtualizadoEm = agendamento.AtualizadoEm;
            await _context.SaveChangesAsync();
        }

        private static Agendamento MapToDomain(AgendamentoModel model)
        {
            var doador = model.Doador is null ? new Doador { Id = model.DoadorId } : new Doador
            {
                Id = model.Doador.Id,
                Nome = model.Doador.Nome,
                Documento = model.Doador.Documento,
                Telefone = model.Doador.Telefone,
                DataNascimento = model.Doador.DataNascimento,
                UsuarioId = model.Doador.UsuarioId,
                HemocentroId = model.Doador.HemocentroId,
                Usuario = model.Doador.UsuarioId.HasValue ? new Usuario { Id = model.Doador.UsuarioId.Value } : null
            };

            var hemocentro = model.Hemocentro is null ? new Hemocentro { Id = model.HemocentroId } : new Hemocentro
            {
                Id = model.Hemocentro.Id,
                Cnpj = model.Hemocentro.Cnpj,
                Site = model.Hemocentro.Site,
                NomeFantasia = model.Hemocentro.NomeFantasia,
                AbreEm = model.Hemocentro.AbreEm,
                FechaEm = model.Hemocentro.FechaEm,
                Usuario = new Usuario { Id = model.Hemocentro.UsuarioId }
            };

            return new Agendamento
            {
                Id = model.Id,
                AgendadoPara = model.AgendadoPara,
                AtualizadoEm = model.AtualizadoEm,
                CriadoEm = model.CriadoEm,
                Status = model.Status,
                Doador = doador,
                Hemocentro = hemocentro
            };
        }
    }
}
