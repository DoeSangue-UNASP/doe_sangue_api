using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Agendamentos
{
    public class ConcluirAgendamentoUseCase
    {
        private readonly IAgendamentoRepository _agendamentoRepository;

        public ConcluirAgendamentoUseCase(IAgendamentoRepository agendamentoRepository)
        {
            _agendamentoRepository = agendamentoRepository;
        }

        public async Task Executar(Guid agendamentoId, Guid usuarioId)
        {
            var agendamento = await _agendamentoRepository.BuscarPorId(agendamentoId);

            if (agendamento is null)
            {
                throw new ArgumentException("Agendamento inexistente.");
            }

            if (agendamento.Hemocentro.Usuario?.Id != usuarioId)
            {
                throw new UnauthorizedAccessException("O usuário não pode concluir este agendamento.");
            }

            agendamento.Concluir();
            await _agendamentoRepository.Atualizar(agendamento);
        }
    }
}
