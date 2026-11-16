using DoeSangue.Application.Dtos.BolsasSangue;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.BolsasSangue
{
    public class CriarBolsaSangueUseCase
    {
        private readonly IBolsaSangueRepository _bolsaSangueRepository;
        private readonly IAgendamentoRepository _agendamentoRepository;

        public CriarBolsaSangueUseCase(IBolsaSangueRepository bolsaSangueRepository, IAgendamentoRepository agendamentoRepository)
        {
            _bolsaSangueRepository = bolsaSangueRepository;
            _agendamentoRepository = agendamentoRepository;
        }

        public async Task Executar(CriarBolsaSangueDto dto)
        {
            var agendamento = await _agendamentoRepository.BuscarPorId(dto.AgendamentoId);
            if (agendamento is null)
            {
                throw new ArgumentException("Agendamento inexistente.");
            }

            var bolsa = BolsaSangue.Criar(
                dto.Codigo,
                dto.TesteSorologico,
                dto.TipoComponente,
                dto.FatorRh,
                dto.TipoSanguineo,
                dto.Volume,
                agendamento);

            await _bolsaSangueRepository.Criar(bolsa);
        }
    }
}
