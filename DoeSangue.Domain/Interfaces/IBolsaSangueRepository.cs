using DoeSangue.Domain.Entities;

namespace DoeSangue.Domain.Interfaces
{
    public interface IBolsaSangueRepository
    {
        Task Criar(BolsaSangue bolsaSangue);
        Task<List<BolsaSangue>> BuscarTodos();
        Task<BolsaSangue?> BuscarPorId(Guid bolsaId);
    }
}
