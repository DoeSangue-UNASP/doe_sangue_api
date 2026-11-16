using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Hemocentros
{
    public class BuscarHemocentroPorUsuarioIdUseCase
    {
        private readonly IHemocentroRepository _hemocentroRepository;

        public BuscarHemocentroPorUsuarioIdUseCase(IHemocentroRepository hemocentroRepository)
        {
            _hemocentroRepository = hemocentroRepository;
        }

        public Task<Hemocentro?> Executar(Guid usuarioId)
        {
            return _hemocentroRepository.BuscarPorUsuarioId(usuarioId);
        }
    }
}
