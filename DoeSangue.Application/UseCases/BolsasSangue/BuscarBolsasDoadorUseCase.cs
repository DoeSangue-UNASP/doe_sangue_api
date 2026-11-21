using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.BolsasSangue
{
    public class BuscarBolsasDoadorUseCase
    {
        private readonly IBolsaSangueRepository _bolsaSangueRepository;

        public BuscarBolsasDoadorUseCase(IBolsaSangueRepository bolsaSangueRepository)
        {
            _bolsaSangueRepository = bolsaSangueRepository;
        }

        public Task<List<BolsaSangue>> Executar(Guid usuarioId)
        {
            return _bolsaSangueRepository.BuscarPorUsuarioDoador(usuarioId);
        }
    }
}
