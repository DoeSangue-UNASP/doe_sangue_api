using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Agendamentos
{
    public class BuscarAgendaHemocentroUseCase
    {
        private readonly IAgendamentoRepository _agendamentoRepository;

        public BuscarAgendaHemocentroUseCase(IAgendamentoRepository agendamentoRepository)
        {
            _agendamentoRepository = agendamentoRepository;
        }

        public Task<List<Agendamento>> Executar(Guid usuarioId, DateOnly? data, string? documento)
        {
            var cpf = string.IsNullOrWhiteSpace(documento)
                ? null
                : new string(documento.Where(char.IsAsciiDigit).ToArray());

            return _agendamentoRepository.BuscarPorUsuarioHemocentro(usuarioId, data, cpf);
        }
    }
}
