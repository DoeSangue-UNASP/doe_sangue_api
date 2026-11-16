using DoeSangue.Application.Dtos.Agendamentos;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Agendamentos
{
    public class CriarAgendamentoUseCase
    {
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly IDoadorRepository _doadorRepository;
        private readonly IHemocentroRepository _hemocentroRepository;

        public CriarAgendamentoUseCase(IAgendamentoRepository agendamentoRepository, IDoadorRepository doadorRepository, IHemocentroRepository hemocentroRepository)
        {
            _agendamentoRepository = agendamentoRepository;
            _doadorRepository = doadorRepository;
            _hemocentroRepository = hemocentroRepository;
        }

        public async Task Executar(CriarAgendamentoDto dto)
        {
            var doador = await _doadorRepository.BuscarPorId(dto.DoadorId);
            if (doador is null)
            {
                throw new ArgumentException("Doador inexistente.");
            }

            var hemocentro = await _hemocentroRepository.BuscarPorId(dto.HemocentroId);
            if (hemocentro is null)
            {
                throw new ArgumentException("Hemocentro inexistente.");
            }

            var agendamento = Agendamento.Criar(dto.AgendadoPara, hemocentro, doador);

            await _agendamentoRepository.Criar(agendamento);
        }
    }
}
