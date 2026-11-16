using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Agendamentos
{
    public class CancelarAgendamentoUseCase
    {
        private readonly IAgendamentoRepository _agendamentoRepository;

        public CancelarAgendamentoUseCase(IAgendamentoRepository agendamentoRepository)
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

            var doadorEhResponsavel = agendamento.Doador.Usuario?.Id == usuarioId;
            var hemocentroEhResponsavel = agendamento.Hemocentro.Usuario?.Id == usuarioId;

            if (!doadorEhResponsavel && !hemocentroEhResponsavel)
            {
                throw new UnauthorizedAccessException("O usuário não pode cancelar este agendamento.");
            }

            agendamento.Cancelar();
            await _agendamentoRepository.Atualizar(agendamento);
        }
    }
}
