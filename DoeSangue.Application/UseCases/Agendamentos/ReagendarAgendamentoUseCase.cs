using DoeSangue.Application.Dtos.Agendamentos;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Agendamentos
{
    public class ReagendarAgendamentoUseCase
    {
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly IHemocentroRepository _hemocentroRepository;

        public ReagendarAgendamentoUseCase(IAgendamentoRepository agendamentoRepository, IHemocentroRepository hemocentroRepository)
        {
            _agendamentoRepository = agendamentoRepository;
            _hemocentroRepository = hemocentroRepository;
        }

        public async Task Executar(Guid agendamentoId, Guid usuarioId, ReagendarAgendamentoDto dto)
        {
            var agendamento = await _agendamentoRepository.BuscarPorId(agendamentoId);

            if (agendamento is null)
            {
                throw new ArgumentException("Agendamento inexistente.");
            }

            if (agendamento.Doador.Usuario?.Id != usuarioId)
            {
                throw new UnauthorizedAccessException("O agendamento deve ser reagendado pelo próprio doador.");
            }

            var hemocentro = await _hemocentroRepository.BuscarPorId(agendamento.Hemocentro.Id);
            if (hemocentro is null)
            {
                throw new ArgumentException("Hemocentro inexistente.");
            }

            var horario = TimeOnly.FromTimeSpan(dto.AgendadoPara.TimeOfDay);
            var abertura = hemocentro.AbreEm;
            var fechamento = hemocentro.FechaEm;
            var horarioDisponivel = fechamento != abertura &&
                (fechamento > abertura
                    ? horario >= abertura && horario < fechamento
                    : horario >= abertura || horario < fechamento);

            if (!horarioDisponivel)
            {
                throw new ArgumentException("O horário solicitado está fora do horário de funcionamento do hemocentro.");
            }

            if (await _agendamentoRepository.ExisteAgendamentoAtivo(hemocentro.Id, dto.AgendadoPara))
            {
                throw new ArgumentException("Já existe um agendamento ativo para esse horário.");
            }

            agendamento.Reagendar(dto.AgendadoPara);
            await _agendamentoRepository.Atualizar(agendamento);
        }
    }
}
