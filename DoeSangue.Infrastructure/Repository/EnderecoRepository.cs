using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;
using DoeSangue.Infrastructure.Data;
using DoeSangue.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace DoeSangue.Infrastructure.Repository
{
    public class EnderecoRepository : IEnderecoRepository
    {
        private readonly DoeSangueContext _context;

        public EnderecoRepository(DoeSangueContext context)
        {
            _context = context;
        }

        public async Task<Endereco?> BuscarPorId(Guid enderecoId)
        {
            var enderecoModel = await _context.Enderecos.FirstOrDefaultAsync(e => e.Id == enderecoId);

            return enderecoModel?.ToDomain();
        }

        public async Task Criar(Endereco endereco)
        {
            var enderecoModel = new EnderecoModel(endereco);

            await _context.Enderecos.AddAsync(enderecoModel);
            await _context.SaveChangesAsync();
        }

        public async Task Remover(Guid enderecoId)
        {
            var enderecoModel = await _context.Enderecos.FirstOrDefaultAsync(e => e.Id == enderecoId);

            if (enderecoModel is null)
            {
                throw new ArgumentException("Endereço inexistente");
            }

            _context.Enderecos.Remove(enderecoModel);
        }
    }
}
