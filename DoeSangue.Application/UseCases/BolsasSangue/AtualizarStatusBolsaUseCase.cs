using DoeSangue.Application.Dtos.BolsasSangue;
using DoeSangue.Domain.Enums;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.BolsasSangue
{
    public class AtualizarStatusBolsaUseCase
    {
        private readonly IBolsaSangueRepository _bolsaSangueRepository;

        public AtualizarStatusBolsaUseCase(IBolsaSangueRepository bolsaSangueRepository)
        {
            _bolsaSangueRepository = bolsaSangueRepository;
        }

        public async Task Executar(Guid bolsaId, Guid usuarioId, AtualizarStatusBolsaDto dto)
        {
            if (!Enum.TryParse<StatusBolsa>(dto.Status, ignoreCase: true, out var novoStatus) ||
                !Enum.IsDefined(novoStatus))
            {
                throw new ArgumentException("Status de bolsa inválido.");
            }

            var bolsa = await _bolsaSangueRepository.BuscarPorId(bolsaId, usuarioId);
            if (bolsa is null)
            {
                throw new ArgumentException("Bolsa de sangue inexistente.");
            }

            bolsa.AtualizarStatus(novoStatus);
            await _bolsaSangueRepository.Atualizar(bolsa);
        }
    }
}
