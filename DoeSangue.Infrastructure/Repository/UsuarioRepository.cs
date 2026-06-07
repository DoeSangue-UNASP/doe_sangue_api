using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;
using DoeSangue.Infrastructure.Data;
using DoeSangue.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DoeSangue.Infrastructure.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DoeSangueContext _context;
        private readonly UserManager<UsuarioIdentity> _userManager;
        public UsuarioRepository(DoeSangueContext context, UserManager<UsuarioIdentity> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task AtualizarEndereco(Endereco endereco)
        {
            var enderecoModel = new EnderecoModel(endereco);

            _context.Enderecos.Attach(enderecoModel);

            _context.Entry(enderecoModel).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task<Usuario?> BuscarPorId(Guid usuarioId)
        {
            var usuarioModel = await _userManager.FindByIdAsync(usuarioId.ToString());

            if (usuarioModel is null) return null;

            var usuario = usuarioModel.ToDomain();

            var enderecoModel = await _context.Enderecos.FirstOrDefaultAsync(x => x.Id == usuarioModel.EnderecoId);
            var endereco = enderecoModel?.ToDomain();

            usuario.AdicionarEndereco(endereco);

            return usuario;
        }

        public async Task Criar(Usuario usuario, string password)
        {
            var usuarioModel = new UsuarioIdentity()
            {
                Id = usuario.Id,
                AtualizadoEm = usuario.AtualizadoEm,
                CriadoEm = usuario.CriadoEm,
                EnderecoId = usuario.Endereco.Id,
                Email = usuario.Email,
                UserName = usuario.Email
            };

            await _userManager.CreateAsync(usuarioModel, password);
            await _userManager.AddToRoleAsync(usuarioModel, usuario.Role.ToString());
        }

        public async Task Remover(Guid usuarioId)
        {
            var usuarioModel = await _userManager.FindByIdAsync(usuarioId.ToString());

            if (usuarioModel is null)
            {
                throw new ArgumentException("Usuário inexistente");
            }

            await _userManager.DeleteAsync(usuarioModel);
        }
    }
}
