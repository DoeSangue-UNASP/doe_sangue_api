using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Hemocentros
{
    public class BuscarTodosHemocentrosUseCase
    {
        private readonly IHemocentroRepository _hemocentroRepository;

        public BuscarTodosHemocentrosUseCase(IHemocentroRepository hemocentroRepository)
        {
            _hemocentroRepository = hemocentroRepository;
        }

        public async Task<List<Hemocentro>> Executar()
        {
            return await _hemocentroRepository.BuscarTodos();
        }
    }
}
