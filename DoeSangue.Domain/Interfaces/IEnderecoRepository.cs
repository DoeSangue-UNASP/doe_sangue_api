using DoeSangue.Domain.Entities;

namespace DoeSangue.Domain.Interfaces
{
    public interface IEnderecoRepository
    {
        Task Criar(Endereco endereco);
        Task<Endereco?> BuscarPorId(Guid enderecoId);
        Task Atualizar(Endereco endereco);
        Task Remover(Guid enderecoId);
    }
}
