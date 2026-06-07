using DoeSangue.Domain.Entities;

namespace DoeSangue.Domain.Interfaces
{
    public interface IDoadorRepository
    {
        Task Criar(Doador doador);
        Task Atualizar(Doador doador);
        Task<Doador?> BuscarPorId(Guid doadorId);
        Task Remover(Guid usuarioId);
    }
}
