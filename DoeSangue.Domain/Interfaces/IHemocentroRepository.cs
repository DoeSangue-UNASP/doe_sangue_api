using DoeSangue.Domain.Entities;

namespace DoeSangue.Domain.Interfaces
{
    public interface IHemocentroRepository
    {
        Task Criar(Hemocentro hemocentro);
        Task Atualizar(Hemocentro hemocentro);
        Task<Hemocentro?> BuscarPorId(Guid hemocentroId);
        Task<Hemocentro?> BuscarPorUsuarioId(Guid usuarioId);
        Task Remover(Guid usuarioId);
    }
}
