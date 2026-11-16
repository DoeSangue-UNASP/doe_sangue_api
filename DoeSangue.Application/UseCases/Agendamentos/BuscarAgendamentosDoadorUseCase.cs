using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Agendamentos
{
    public class BuscarAgendamentosDoadorUseCase
    {
        private readonly IAgendamentoRepository _agendamentoRepository;

        public BuscarAgendamentosDoadorUseCase(IAgendamentoRepository agendamentoRepository)
        {
            _agendamentoRepository = agendamentoRepository;
        }

        public Task<List<Agendamento>> Executar(Guid usuarioId)
        {
            return _agendamentoRepository.BuscarPorUsuarioDoador(usuarioId);
        }
    }
}
