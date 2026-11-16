using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Depoimentos
{
    public class BuscarDepoimentosUseCase
    {
        private readonly IDepoimentoRepository _depoimentoRepository;

        public BuscarDepoimentosUseCase(IDepoimentoRepository depoimentoRepository)
        {
            _depoimentoRepository = depoimentoRepository;
        }

        public async Task<List<Depoimento>> Executar()
        {
            return await _depoimentoRepository.BuscarTodos();
        }
    }
}
