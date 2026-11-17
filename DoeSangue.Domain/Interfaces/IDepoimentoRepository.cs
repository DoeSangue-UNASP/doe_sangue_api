using DoeSangue.Domain.Entities;

namespace DoeSangue.Domain.Interfaces
{
    public interface IDepoimentoRepository
    {
        Task Criar(Depoimento depoimento);
        Task<List<Depoimento>> BuscarTodos();
        Task<List<Depoimento>> BuscarDezMaisRecentes();
        Task<Depoimento?> BuscarPorId(Guid depoimentoId);
    }
}
