using DoeSangue.Domain.Entities;

namespace DoeSangue.Domain.Interfaces
{
    public interface IDoadorRepository
    {
        Task Criar(Doador doador);
        Task Atualizar(Doador doador);
        Task<Doador?> BuscarPorId(Guid doadorId);
        Task<Doador?> BuscarPorUsuarioId(Guid usuarioId);
        Task<Doador?> BuscarPorIdDoHemocentro(Guid doadorId, Guid usuarioHemocentroId);
        Task<List<Doador>> BuscarTodosDoHemocentro(Guid usuarioHemocentroId);
        Task Remover(Guid usuarioId);
    }
}
