using DoeSangue.Domain.Entities;

namespace DoeSangue.Domain.Interfaces
{
    public interface IAgendamentoRepository
    {
        Task Criar(Agendamento agendamento);
        Task<Agendamento?> BuscarPorId(Guid agendamentoId);
        Task<List<Agendamento>> BuscarPorUsuarioDoador(Guid usuarioId);
        Task<List<Agendamento>> BuscarPorUsuarioHemocentro(Guid usuarioId, DateOnly? data, string? documento);
        Task<bool> ExisteAgendamentoAtivo(Guid hemocentroId, DateTime agendadoPara);
        Task Atualizar(Agendamento agendamento);
    }
}
