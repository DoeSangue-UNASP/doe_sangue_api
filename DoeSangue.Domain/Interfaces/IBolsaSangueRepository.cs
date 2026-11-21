using DoeSangue.Domain.Entities;

namespace DoeSangue.Domain.Interfaces
{
    public interface IBolsaSangueRepository
    {
        Task Criar(BolsaSangue bolsaSangue);
        Task<List<BolsaSangue>> BuscarPorUsuarioHemocentro(Guid usuarioId);
        Task<List<BolsaSangue>> BuscarPorUsuarioDoador(Guid usuarioId);
        Task<BolsaSangue?> BuscarPorId(Guid bolsaId, Guid usuarioId);
        Task Atualizar(BolsaSangue bolsaSangue);
    }
}
