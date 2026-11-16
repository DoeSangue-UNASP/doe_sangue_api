using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Doadores
{
    public class BuscarDoadorPorUsuarioIdUseCase
    {
        private readonly IDoadorRepository _doadorRepository;

        public BuscarDoadorPorUsuarioIdUseCase(IDoadorRepository doadorRepository)
        {
            _doadorRepository = doadorRepository;
        }

        public Task<Doador?> Executar(Guid usuarioId)
        {
            return _doadorRepository.BuscarPorUsuarioId(usuarioId);
        }
    }
}
