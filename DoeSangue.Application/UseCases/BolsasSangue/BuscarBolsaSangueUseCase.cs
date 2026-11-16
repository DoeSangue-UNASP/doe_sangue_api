using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.BolsasSangue
{
    public class BuscarBolsaSangueUseCase
    {
        private readonly IBolsaSangueRepository _bolsaSangueRepository;

        public BuscarBolsaSangueUseCase(IBolsaSangueRepository bolsaSangueRepository)
        {
            _bolsaSangueRepository = bolsaSangueRepository;
        }

        public Task<BolsaSangue?> Executar(Guid bolsaId, Guid usuarioId)
        {
            return _bolsaSangueRepository.BuscarPorId(bolsaId, usuarioId);
        }
    }
}
