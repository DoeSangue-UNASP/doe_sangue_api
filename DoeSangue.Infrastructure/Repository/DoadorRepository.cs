using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;
using DoeSangue.Infrastructure.Data;
using DoeSangue.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DoeSangue.Infrastructure.Repository
{
    public class DoadorRepository : IDoadorRepository
    {
        private readonly DoeSangueContext _context;
        private readonly UserManager<UsuarioIdentity> _userManager;

        public DoadorRepository(DoeSangueContext context, UserManager<UsuarioIdentity> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task Atualizar(Doador doador)
        {
            var doadorModel = new DoadorModel(doador);
            var atualizadoEm = DateTime.Now;

            var usuarioModel = await _userManager.FindByIdAsync(doadorModel.UsuarioId.ToString());

            if (usuarioModel is null)
            {
                throw new ArgumentException("Usuário inexistente");
            }

            _context.Doadores.Attach(doadorModel);
            _context.Entry(doadorModel).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            usuarioModel.AtualizadoEm = atualizadoEm;
            await _userManager.UpdateAsync(usuarioModel);
        }

        public async Task<Doador?> BuscarPorId(Guid doadorId)
        {
            var doadorModel = await _context.Doadores.AsNoTracking().FirstOrDefaultAsync(d => d.Id == doadorId);
            
            if (doadorModel is null) return null;
            
            var doador = doadorModel.ToDomain();

            var usuarioId = doadorModel.UsuarioId;
            var usuarioModel = await _userManager.FindByIdAsync(usuarioId.ToString());

            if (usuarioModel is null)
            {
                throw new Exception("Usuário inexistente");
            }

            var usuario = usuarioModel.ToDomain();

            doador.AdicionarUsuario(usuario);

            return doador;
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
