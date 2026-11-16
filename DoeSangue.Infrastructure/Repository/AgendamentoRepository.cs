using DoeSangue.Domain.Entities;
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
            var agendamentoModel = new AgendamentoModel(
                agendamento.Id,
                agendamento.AtualizadoEm,
                agendamento.CriadoEm,
                agendamento.AgendadoPara,
                agendamento.Status,
                agendamento.Hemocentro.Id,
                new HemocentroModel(agendamento.Hemocentro),
                agendamento.Doador.Id);

            await _context.Agendamentos.AddAsync(agendamentoModel);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Agendamento>> BuscarTodos()
        {
            var models = await _context.Agendamentos
                .Include(x => x.Doador)
                .Include(x => x.Hemocentro)
                .AsNoTracking()
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

        private static Agendamento MapToDomain(AgendamentoModel model)
        {
            var doador = model.Doador is null ? new Doador { Id = model.DoadorId } : new Doador
            {
                Id = model.Doador.Id,
                Nome = model.Doador.Nome,
                Documento = model.Doador.Documento,
                Telefone = model.Doador.Telefone,
                DataNascimento = model.Doador.DataNascimento,
                Usuario = new Usuario { Id = model.Doador.UsuarioId }
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
