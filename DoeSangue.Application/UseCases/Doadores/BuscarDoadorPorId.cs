using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Doadores
{
    public class BuscarDoadorPorId
    {
        private readonly IDoadorRepository _doadorRepository;

        public BuscarDoadorPorId(IDoadorRepository doadorRepository)
        {
            _doadorRepository = doadorRepository;
        }

        public async Task<Doador?> Executar(Guid doadorId, Guid usuarioHemocentroId)
        {
            return await _doadorRepository.BuscarPorIdDoHemocentro(doadorId, usuarioHemocentroId);
        }
    }
}
