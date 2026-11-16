using DoeSangue.Domain.Entities;

namespace DoeSangue.Domain.Interfaces
{
    public interface IAgendamentoRepository
    {
        Task Criar(Agendamento agendamento);
        Task<List<Agendamento>> BuscarTodos();
        Task<Agendamento?> BuscarPorId(Guid agendamentoId);
    }
}
