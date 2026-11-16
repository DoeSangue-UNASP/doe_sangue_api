using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.BolsasSangue
{
    public class BuscarBolsasSangueUseCase
    {
        private readonly IBolsaSangueRepository _bolsaSangueRepository;

        public BuscarBolsasSangueUseCase(IBolsaSangueRepository bolsaSangueRepository)
        {
            _bolsaSangueRepository = bolsaSangueRepository;
        }

        public Task<List<BolsaSangue>> Executar(Guid usuarioId)
        {
            return _bolsaSangueRepository.BuscarPorUsuarioHemocentro(usuarioId);
        }
    }
}
