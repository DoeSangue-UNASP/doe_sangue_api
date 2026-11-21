using DoeSangue.Application.Dtos.BolsasSangue;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Enums;
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

        public async Task Executar(Guid usuarioId, CriarBolsaSangueDto dto)
        {
            var agendamento = await _agendamentoRepository.BuscarPorId(dto.AgendamentoId);
            if (agendamento is null)
            {
                throw new ArgumentException("Agendamento inexistente.");
            }

            if (agendamento.Hemocentro.Usuario?.Id != usuarioId)
            {
                throw new UnauthorizedAccessException("O usuário não pode cadastrar uma bolsa para este agendamento.");
            }

            if (agendamento.Status != StatusAgendamento.CONCLUIDO)
            {
                throw new ArgumentException("A bolsa só pode ser cadastrada após a conclusão do agendamento.");
            }

            var bolsa = BolsaSangue.Criar(
                dto.Codigo,
                dto.TesteSorologico,
                dto.TipoComponente,
                dto.FatorRh,
                dto.TipoSanguineo,
                dto.Volume,
                agendamento,
                dto.DataColeta,
                dto.DataValidade,
                dto.LocalColeta,
                dto.Destino,
                dto.ResponsavelColeta);

            await _bolsaSangueRepository.Criar(bolsa);
        }
    }
}
