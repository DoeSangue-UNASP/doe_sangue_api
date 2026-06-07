using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;
using DoeSangue.Infrastructure.Data;
using DoeSangue.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DoeSangue.Infrastructure.Repository
{
    public class HemocentroRepository : IHemocentroRepository
    {
        private readonly DoeSangueContext _context;
        private readonly UserManager<UsuarioIdentity> _userManager;

        public HemocentroRepository(DoeSangueContext context, UserManager<UsuarioIdentity> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task Atualizar(Hemocentro hemocentro)
        {
            var hemocentroModel = new HemocentroModel(hemocentro);
            var atualizadoEm = DateTime.Now;

            var usuarioModel = await _userManager.FindByIdAsync(hemocentroModel.UsuarioId.ToString());

            if (usuarioModel is null)
            {
                throw new ArgumentException("Usuário inexistente");
            }

            _context.Hemocentros.Attach(hemocentroModel);
            _context.Entry(hemocentroModel).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            usuarioModel.AtualizadoEm = atualizadoEm;
            await _userManager.UpdateAsync(usuarioModel);
        }

        public async Task<Hemocentro?> BuscarPorId(Guid hemocentroId)
        {
            var hemocentroModel = await _context.Hemocentros.FirstOrDefaultAsync(x => x.Id == hemocentroId);

            if (hemocentroModel is null) return null;

            var hemocentro = hemocentroModel.ToDomain();

            var usuarioId = hemocentroModel.UsuarioId;
            var usuarioModel = await _userManager.FindByIdAsync(usuarioId.ToString());

            if (usuarioModel is null)
            {
                throw new Exception("Usuário inexistente");
            }

            var usuario = usuarioModel.ToDomain();

            hemocentro.AdicionarUsuario(usuario);

            return hemocentro;
        }

        public async Task Criar(Hemocentro hemocentro)
        {
            var hemocentroModel = new HemocentroModel(hemocentro);

            await _context.Hemocentros.AddAsync(hemocentroModel);
            await _context.SaveChangesAsync();
        }

        public async Task Remover(Guid usuarioId)
        {
            var hemocentroModel = await _context.Hemocentros.FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);

            if (hemocentroModel is null)
            {
                throw new ArgumentException("Hemocentro inexistente");
            }

            _context.Hemocentros.Remove(hemocentroModel);
            await _context.SaveChangesAsync();
        }
    }
}
