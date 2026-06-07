using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;
using DoeSangue.Infrastructure.Data;
using DoeSangue.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace DoeSangue.Infrastructure.Repository
{
    public class DoadorRepository : IDoadorRepository
    {
        private readonly DoeSangueContext _context;

        public DoadorRepository(DoeSangueContext context)
        {
            _context = context;
        }

        public Task Atualizar(Doador doador)
        {
            throw new NotImplementedException();
        }

        public async Task<Doador?> BuscarPorId(Guid doadorId)
        {
            var doadorModel = await _context.Doadores.FirstOrDefaultAsync(d => d.Id == doadorId);

            return doadorModel?.ToDomain();
        }

        public async Task Criar(Doador doador)
        {
            var doadorModel = new DoadorModel(doador);

            await _context.Doadores.AddAsync(doadorModel);
            await _context.SaveChangesAsync();
        }

        public async Task Remover(Guid usuarioId)
        {
            var doadorModel = await _context.Doadores.FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);

            if (doadorModel is null)
            {
                throw new ArgumentException("Doador inexistente");
            }

            _context.Doadores.Remove(doadorModel);
            await _context.SaveChangesAsync();
        }
    }
}
