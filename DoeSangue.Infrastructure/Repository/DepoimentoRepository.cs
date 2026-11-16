using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;
using DoeSangue.Infrastructure.Data;
using DoeSangue.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DoeSangue.Infrastructure.Repository
{
    public class DepoimentoRepository : IDepoimentoRepository
    {
        private readonly DoeSangueContext _context;
        private readonly UserManager<UsuarioIdentity> _userManager;

        public DepoimentoRepository(DoeSangueContext context, UserManager<UsuarioIdentity> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task Criar(Depoimento depoimento)
        {
            var autorId = depoimento.Autor.Id;

            var autorModel = await _userManager.FindByIdAsync(autorId.ToString());

            if (autorModel is null)
            {
                throw new ArgumentException("Autor do depoimento não encontrado.");
            }

            var model = new DepoimentoModel
            {
                Id = depoimento.Id,
                Titulo = depoimento.Titulo,
                Conteudo = depoimento.Conteudo,
                CriadoEm = depoimento.CriadoEm,
                AtualizadoEm = depoimento.AtualizadoEm,
                AutorId = autorId
            };

            await _context.Depoimentos.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Depoimento>> BuscarTodos()
        {
            var models = await _context.Depoimentos
                .AsNoTracking()
                .Include(x => x.Autor)
                .OrderByDescending(x => x.CriadoEm)
                .ToListAsync();

            var depoimentos = new List<Depoimento>();

            foreach (var model in models)
            {
                var autor = model.Autor?.ToDomain() ?? new Usuario { Id = model.AutorId };

                depoimentos.Add(new Depoimento
                {
                    Id = model.Id,
                    Titulo = model.Titulo,
                    Conteudo = model.Conteudo,
                    CriadoEm = model.CriadoEm,
                    AtualizadoEm = model.AtualizadoEm,
                    Autor = autor
                });
            }

            return depoimentos;
        }

        public async Task<Depoimento?> BuscarPorId(Guid depoimentoId)
        {
            var model = await _context.Depoimentos
                .AsNoTracking()
                .Include(x => x.Autor)
                .FirstOrDefaultAsync(x => x.Id == depoimentoId);

            if (model is null)
            {
                return null;
            }

            var autor = model.Autor?.ToDomain() ?? new Usuario { Id = model.AutorId };

            return new Depoimento
            {
                Id = model.Id,
                Titulo = model.Titulo,
                Conteudo = model.Conteudo,
                CriadoEm = model.CriadoEm,
                AtualizadoEm = model.AtualizadoEm,
                Autor = autor
            };
        }
    }
}
