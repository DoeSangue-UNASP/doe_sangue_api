using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;
using DoeSangue.Infrastructure.Data;
using DoeSangue.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace DoeSangue.Infrastructure.Repository
{
    public class BolsaSangueRepository : IBolsaSangueRepository
    {
        private readonly DoeSangueContext _context;

        public BolsaSangueRepository(DoeSangueContext context)
        {
            _context = context;
        }

        public async Task Criar(BolsaSangue bolsaSangue)
        {
            var bolsaModel = new BolsaSangueModel(
                bolsaSangue.Id,
                bolsaSangue.Codigo,
                bolsaSangue.TesteSorologico,
                bolsaSangue.TipoComponente,
                bolsaSangue.FatorRh,
                bolsaSangue.TipoSanguineo,
                bolsaSangue.Volume,
                bolsaSangue.Status,
                bolsaSangue.CriadoEm,
                bolsaSangue.AtualizadoEm,
                bolsaSangue.Agendamento.Id);

            await _context.BolsasSangue.AddAsync(bolsaModel);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BolsaSangue>> BuscarPorUsuarioHemocentro(Guid usuarioId)
        {
            var models = await _context.BolsasSangue
                .Include(x => x.Agendamento)
                .ThenInclude(x => x.Doador)
                .Include(x => x.Agendamento)
                .ThenInclude(x => x.Hemocentro)
                .Where(x => x.Agendamento.Hemocentro.UsuarioId == usuarioId)
                .AsNoTracking()
                .ToListAsync();

            return models.Select(MapToDomain).ToList();
        }

        public async Task<BolsaSangue?> BuscarPorId(Guid bolsaId, Guid usuarioId)
        {
            var model = await _context.BolsasSangue
                .Include(x => x.Agendamento)
                .ThenInclude(x => x.Doador)
                .Include(x => x.Agendamento)
                .ThenInclude(x => x.Hemocentro)
                .FirstOrDefaultAsync(x =>
                    x.Id == bolsaId &&
                    x.Agendamento.Hemocentro.UsuarioId == usuarioId);

            if (model is null)
            {
                return null;
            }

            return MapToDomain(model);
        }

        public async Task Atualizar(BolsaSangue bolsaSangue)
        {
            var model = await _context.BolsasSangue
                .FirstOrDefaultAsync(x => x.Id == bolsaSangue.Id);

            if (model is null)
            {
                throw new ArgumentException("Bolsa de sangue inexistente.");
            }

            model.Status = bolsaSangue.Status;
            model.AtualizadoEm = bolsaSangue.AtualizadoEm;
            await _context.SaveChangesAsync();
        }

        private static BolsaSangue MapToDomain(BolsaSangueModel model)
        {
            var agendamento = new Agendamento
            {
                Id = model.AgendamentoId,
                AgendadoPara = model.Agendamento?.AgendadoPara ?? DateTime.UtcNow,
                CriadoEm = model.Agendamento?.CriadoEm ?? DateTime.UtcNow,
                Status = model.Agendamento?.Status ?? default,
                Doador = model.Agendamento?.Doador is null ? new Doador { Id = model.Agendamento!.DoadorId } : new Doador
                {
                    Id = model.Agendamento.Doador.Id,
                    Nome = model.Agendamento.Doador.Nome,
                    Documento = model.Agendamento.Doador.Documento,
                    Telefone = model.Agendamento.Doador.Telefone,
                    DataNascimento = model.Agendamento.Doador.DataNascimento,
                    Usuario = new Usuario { Id = model.Agendamento.Doador.UsuarioId }
                },
                Hemocentro = model.Agendamento?.Hemocentro is null ? new Hemocentro { Id = model.Agendamento!.HemocentroId } : new Hemocentro
                {
                    Id = model.Agendamento.Hemocentro.Id,
                    Cnpj = model.Agendamento.Hemocentro.Cnpj,
                    Site = model.Agendamento.Hemocentro.Site,
                    NomeFantasia = model.Agendamento.Hemocentro.NomeFantasia,
                    AbreEm = model.Agendamento.Hemocentro.AbreEm,
                    FechaEm = model.Agendamento.Hemocentro.FechaEm,
                    Usuario = new Usuario { Id = model.Agendamento.Hemocentro.UsuarioId }
                }
            };

            return new BolsaSangue
            {
                Id = model.Id,
                Codigo = model.Codigo,
                TesteSorologico = model.TesteSorologico,
                TipoComponente = model.TipoComponente,
                FatorRh = model.FatorRh,
                TipoSanguineo = model.TipoSanguineo,
                Volume = model.Volume,
                Status = model.Status,
                CriadoEm = model.CriadoEm,
                AtualizadoEm = model.AtualizadoEm,
                Agendamento = agendamento
            };
        }
    }
}
