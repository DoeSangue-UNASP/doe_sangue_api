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

            _context.Doadores.Attach(doadorModel);
            _context.Entry(doadorModel).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            if (doadorModel.UsuarioId.HasValue)
            {
                var usuarioModel = await _userManager.FindByIdAsync(doadorModel.UsuarioId.ToString());
                if (usuarioModel is not null)
                {
                    usuarioModel.AtualizadoEm = atualizadoEm;
                    await _userManager.UpdateAsync(usuarioModel);
                }
            }
        }

        public async Task<Doador?> BuscarPorId(Guid doadorId)
        {
            var doadorModel = await _context.Doadores.AsNoTracking().FirstOrDefaultAsync(d => d.Id == doadorId);
            
            if (doadorModel is null) return null;
            
            var doador = doadorModel.ToDomain();

            if (doadorModel.UsuarioId.HasValue)
            {
                var usuarioModel = await _userManager.FindByIdAsync(doadorModel.UsuarioId.ToString());
                if (usuarioModel is not null)
                {
                    doador.AdicionarUsuario(await MapearUsuarioComEndereco(usuarioModel));
                }
            }

            if (doadorModel.HemocentroId.HasValue)
            {
                var hemocentroModel = await _context.Hemocentros.FirstOrDefaultAsync(h => h.Id == doadorModel.HemocentroId);
                if (hemocentroModel is not null)
                {
                    doador.AdicionarHemocentro(hemocentroModel.ToDomain());
                }
            }

            return doador;
        }

        public async Task<Doador?> BuscarPorUsuarioId(Guid usuarioId)
        {
            var doadorModel = await _context.Doadores
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);

            if (doadorModel is null)
            {
                return null;
            }

            var doador = doadorModel.ToDomain();
            var usuarioModel = await _userManager.FindByIdAsync(usuarioId.ToString());

            if (usuarioModel is null)
            {
                throw new InvalidOperationException("Usuário associado ao doador não foi encontrado.");
            }

            doador.AdicionarUsuario(await MapearUsuarioComEndereco(usuarioModel));
            return doador;
        }

        private async Task<Usuario> MapearUsuarioComEndereco(UsuarioIdentity usuarioModel)
        {
            var usuario = usuarioModel.ToDomain();
            var enderecoModel = await _context.Enderecos
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == usuarioModel.EnderecoId);

            if (enderecoModel is not null)
            {
                usuario.AdicionarEndereco(enderecoModel.ToDomain());
            }

            return usuario;
        }

        // Doadores usuários são visíveis para qualquer hemocentro; leads, apenas para o hemocentro que os cadastrou
        public async Task<Doador?> BuscarPorIdDoHemocentro(Guid doadorId, Guid usuarioHemocentroId)
        {
            var doadorModel = await _context.Doadores
                .AsNoTracking()
                .FirstOrDefaultAsync(d =>
                    d.Id == doadorId &&
                    (d.UsuarioId != null ||
                     _context.Hemocentros.Any(h => h.Id == d.HemocentroId && h.UsuarioId == usuarioHemocentroId)));

            if (doadorModel is null)
            {
                return null;
            }

            var doador = doadorModel.ToDomain();
            if (doadorModel.UsuarioId.HasValue)
            {
                var usuarioModel = await _userManager.FindByIdAsync(doadorModel.UsuarioId.ToString());
                if (usuarioModel is not null)
                {
                    doador.AdicionarUsuario(usuarioModel.ToDomain());
                }
            }

            return doador;
        }

        // Todos os doadores usuários + os leads cadastrados pelo hemocentro.
        // Um lead convertido em usuário pode coexistir com a linha do próprio usuário; mantém uma por pessoa,
        // preferindo a linha do próprio usuário.
        public async Task<List<Doador>> BuscarTodosDoHemocentro(Guid usuarioHemocentroId)
        {
            var models = await _context.Doadores
                .Where(d =>
                    d.UsuarioId != null ||
                    _context.Hemocentros.Any(h => h.Id == d.HemocentroId && h.UsuarioId == usuarioHemocentroId))
                .AsNoTracking()
                .OrderBy(x => x.Nome)
                .ToListAsync();

            return models
                .GroupBy(x => x.UsuarioId ?? x.Id)
                .Select(grupo => grupo.OrderBy(x => x.HemocentroId.HasValue).First().ToDomain())
                .ToList();
        }

        public async Task<List<Doador>> BuscarLeadsEUsuariosDoHemocentro(Guid hemocentroId)
        {
            var models = await _context.Doadores
                .Where(d => d.HemocentroId == hemocentroId || _context.Agendamentos.Any(a =>
                    a.DoadorId == d.Id &&
                    a.Hemocentro.Id == hemocentroId))
                .AsNoTracking()
                .OrderBy(x => x.Nome)
                .ToListAsync();

            var doadores = new List<Doador>();
            foreach (var model in models)
            {
                var doador = model.ToDomain();

                if (model.UsuarioId.HasValue)
                {
                    var usuarioModel = await _userManager.FindByIdAsync(model.UsuarioId.ToString());
                    if (usuarioModel is not null)
                    {
                        doador.AdicionarUsuario(usuarioModel.ToDomain());
                    }
                }

                if (model.HemocentroId.HasValue)
                {
                    var hemocentroModel = await _context.Hemocentros.FirstOrDefaultAsync(h => h.Id == model.HemocentroId);
                    if (hemocentroModel is not null)
                    {
                        doador.AdicionarHemocentro(hemocentroModel.ToDomain());
                    }
                }

                doadores.Add(doador);
            }

            return doadores;
        }

        public async Task<List<Doador>?> BuscarPorDocumento(string documento)
        {
            // Compara apenas os dígitos, pois o CPF pode ter sido salvo com ou sem máscara
            var digitos = new string(documento.Where(char.IsDigit).ToArray());
            if (digitos.Length == 0) return null;

            var models = await _context.Doadores
                .AsNoTracking()
                .Where(d => d.Documento.Replace(".", "").Replace("-", "").Replace(" ", "") == digitos)
                .ToListAsync();

            if (!models.Any()) return null;

            var doadores = new List<Doador>();
            foreach (var model in models)
            {
                var doador = model.ToDomain();

                if (model.UsuarioId.HasValue)
                {
                    var usuarioModel = await _userManager.FindByIdAsync(model.UsuarioId.ToString());
                    if (usuarioModel is not null)
                    {
                        doador.AdicionarUsuario(usuarioModel.ToDomain());
                    }
                }

                if (model.HemocentroId.HasValue)
                {
                    var hemocentroModel = await _context.Hemocentros.FirstOrDefaultAsync(h => h.Id == model.HemocentroId);
                    if (hemocentroModel is not null)
                    {
                        doador.AdicionarHemocentro(hemocentroModel.ToDomain());
                    }
                }

                doadores.Add(doador);
            }

            return doadores;
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

