using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Doadores
{
    public class BuscarTodosDoadoresUseCase
    {
        private readonly IDoadorRepository _doadorRepository;

        public BuscarTodosDoadoresUseCase(IDoadorRepository doadorRepository)
        {
            _doadorRepository = doadorRepository;
        }

        public Task<List<Doador>> Executar(Guid usuarioHemocentroId)
        {
            return _doadorRepository.BuscarTodosDoHemocentro(usuarioHemocentroId);
        }
    }
}
